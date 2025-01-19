using ActivityTracker.Data;
using ActivityTracker.Models.Response_Models;
using ActivityTracker.Models;
using System.Data.Entity;
using ActivityTracker.Models.Request_Models;

namespace ActivityTracker.Services
{
    public class TrainingService
    {
        private readonly ApplicationDbContext _context;

        public TrainingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Training> CreateTrainingAsync(string email, TrainingStartDto trainingDto)
        {

            var user = _context.Users.Where(u => u.Email == email).FirstOrDefault();
            var training = new Training
            {
                StartTime = DateTime.Now,
                UserId = user.Id,
                Type = trainingDto.type,
            };
            _context.Trainings.Add(training);
            await _context.SaveChangesAsync();
            return training;
        }

        public async Task<List<TrainingDetailDto>> GetMyTrainingsAsync(string email)
        {
            var user =  _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return new List<TrainingDetailDto>();
            }

            var trainings = _context.Trainings
                .Include(t => t.TrainingPoints)
                .Where(t => t.UserId == user.Id)
                .ToList();

            var trainingDetailsList = new List<TrainingDetailDto>();

            foreach (var training in trainings)
            {
                var points = _context.TrainingPoints.Where(tp => tp.TrainingId == training.Id).ToList();

                // Map points to DTOs
                var pointsDtos = points.Select(tp => new TrainingPointDto
                {
                    Latitude = tp.Latitude,
                    Longitude = tp.Longitude,
                    PointTime = tp.PointTime
                }).ToList();

                // Calculate derived metrics
                var totalTime = (training.EndTime - training.StartTime)?.TotalSeconds ?? 0;
                var distanceCovered = CalculateTotalDistance(points); // Implement your logic as in GetTrainingDetailsAsync
                var topSpeed = training.TopSpeed;
                var avgSpeed = totalTime > 0 ? (distanceCovered / (totalTime / 3600)) : 0;
                var caloriesBurned = CalculateCaloriesBurned(user, training, distanceCovered); // Same logic as in GetTrainingDetailsAsync

                // Construct the DTO
                var dto = new TrainingDetailDto
                {
                    Id = training.Id,
                    StartTime = training.StartTime,
                    EndTime = training.EndTime,
                    Type = training.Type,
                    TotalTime = totalTime,
                    TopSpeed = topSpeed,
                    AverageSpeed = avgSpeed,
                    DistanceCovered = distanceCovered,
                    CaloriesBurned = caloriesBurned,
                    Points = pointsDtos
                };

                trainingDetailsList.Add(dto);
            }

            return trainingDetailsList;
        }


        public async Task<TrainingDetailDto> GetTrainingDetailsAsync(int trainingId)
        {
            var training = _context.Trainings.Where(t => t.Id == trainingId)
                .Include(t => t.TrainingPoints)
                .FirstOrDefault();
            var points = _context.TrainingPoints.Where(tp => tp.TrainingId == training.Id).ToList();
            var pointsDtos = _context.TrainingPoints.Where(tp => tp.TrainingId == training.Id).Select(tp => new TrainingPointDto { Latitude = tp.Latitude, Longitude = tp.Longitude, PointTime = tp.PointTime }).ToList();
            if (training == null) return null;

            var totalTime = (training.EndTime - training.StartTime)?.TotalSeconds ?? 0; // Total time in seconds

            var distanceCovered = CalculateTotalDistance(points); // Total distance covered in km
            var topSpeed = training.TopSpeed; // Top speed recorded during the training in km/h

            // Calculate average speed (distance / total time in hours)
            var avgSpeed = totalTime > 0 ? (distanceCovered / (totalTime / 3600)) : 0; // Convert totalTime to hours

            // Calculate calories burned
            var user = await _context.Users.FindAsync(training.UserId);
            var caloriesBurned = user != null ? CalculateCaloriesBurned(user, training, distanceCovered) : 0.0;

            // Create the DTO to return
            return new TrainingDetailDto
            {
                Id = training.Id,
                StartTime = training.StartTime,
                EndTime = training.EndTime,
                Type = training.Type,
                TotalTime = totalTime,
                TopSpeed = topSpeed,
                AverageSpeed = avgSpeed,
                DistanceCovered = distanceCovered,
                CaloriesBurned = caloriesBurned,
                Points = pointsDtos
            };
        }

        public async Task FinishTrainingAsync(int trainingId)
        {
            var training = _context.Trainings
                .Include(t => t.TrainingPoints)
                .FirstOrDefault(t => t.Id == trainingId);

            if (training == null) return;
            training.EndTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Calculate the calories burned
            var user = await _context.Users.FindAsync(training.UserId);
            if (user != null)
            {
                training.CaloriesBurned = CalculateCaloriesBurned(user, training, training.DistanceCovered ?? 0);
            }

            await _context.SaveChangesAsync();
        }
        private double CalculateTotalDistance(IEnumerable<TrainingPoint> trainingPoints)
        {
            double totalDistance = 0;
            var points = trainingPoints.OrderBy(tp => tp.PointTime).ToList();

            for (int i = 1; i < points.Count; i++)
            {
                var previousPoint = points[i - 1];
                var currentPoint = points[i];
                totalDistance += CalculateDistanceBetweenPoints(previousPoint, currentPoint);
            }

            return totalDistance;
        }
        private double CalculateDistanceBetweenPoints(TrainingPoint p1, TrainingPoint p2)
        {
            const double R = 6371; // Earth's radius in kilometers
            var lat1 = p1.Latitude * Math.PI / 180;
            var lon1 = p1.Longitude * Math.PI / 180;
            var lat2 = p2.Latitude * Math.PI / 180;
            var lon2 = p2.Longitude * Math.PI / 180;

            var dlat = lat2 - lat1;
            var dlon = lon2 - lon1;

            var a = Math.Pow(Math.Sin(dlat / 2), 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Pow(Math.Sin(dlon / 2), 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in kilometers
        }
        private double CalculateCaloriesBurned(ApplicationUser user, Training training, double totalDistance)
        {
            double weightInKg = string.IsNullOrEmpty(user.Weight) ? 70 : double.Parse(user.Weight); // Default to 70kg if not specified
            double durationInHours = training.EndTime != null ? (double)((training?.EndTime - training.StartTime)?.TotalHours) : 0;

            // Calculate average speed in km/h
            double avgSpeed = (double)(totalDistance / durationInHours);

            double mets = GetMetsValue(training.Type, avgSpeed);

            return mets * weightInKg * durationInHours;
        }

        private double GetMetsValue(TrainingType type, double avgSpeed)
        {
            if (type == TrainingType.Running)
            {
                if (avgSpeed <= 8) return 7.0; 
                else if (avgSpeed <= 10) return 9.0; 
                else if (avgSpeed <= 12) return 11.5; 
                else return 13.5; 
            }
            else if (type == TrainingType.Cycling)
            {
                if (avgSpeed <= 12) return 4.0;
                else if (avgSpeed <= 16) return 6.0;
                else if (avgSpeed <= 19) return 8.0;
                else return 10.0;
            }

            return 1.0;
        }

        public async Task<TrainingStatsDto> GetTrainingStatsAsync(string email, TrainingType? type, DateTime startDate, DateTime endDate)
        {
            // Find the user by email
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            // Fetch trainings based on the type and date range

            var trainings = new List<Training>();
            if(type == null)
            trainings = _context.Trainings
                .Where(t => t.UserId == user.Id &&
                            t.StartTime >= startDate) // Include all if type is null, filter otherwise
                .ToList();
            if(type != null)
                trainings = _context.Trainings
                    .Where(t => t.UserId == user.Id &&
                                t.StartTime >= startDate &&
                                t.Type == type) // Include all if type is null, filter otherwise
                    .ToList();
            // Calculate statistics
            var totalDistance = trainings.Sum(t => t.DistanceCovered ?? 0);
            var totalTime = trainings.Sum(t => (t.EndTime - t.StartTime)?.TotalHours ?? 0);
            var totalCalories = trainings.Sum(t => t.CaloriesBurned ?? 0);
            var topSpeed = trainings.Select(t => t.TopSpeed ?? 0).DefaultIfEmpty(0).Max();
            var longestSession = trainings.Select(t => (t.EndTime - t.StartTime) ?? TimeSpan.Zero).DefaultIfEmpty(TimeSpan.Zero).Max();
            var longestRoute = trainings.Select(t => t.DistanceCovered ?? 0).DefaultIfEmpty(0).Max();
            var avgTimeBetweenSessions = CalculateAverageTimeBetweenSessions(trainings);

            // Return stats
            return new TrainingStatsDto
            {
                NumberOfTrainings = trainings.Count,
                TotalTimeSpent = totalTime,
                TotalDistanceCovered = totalDistance,
                TotalCaloriesBurned = totalCalories,
                TopSpeed = topSpeed,
                LongestSession = longestSession,
                LongestRouteCovered = longestRoute,
                AvgTimeBetweenSessions = avgTimeBetweenSessions
            };
        }

        private double CalculateAverageTimeBetweenSessions(List<Training> trainings)
        {
            if (trainings.Count < 2) return 0;

            var orderedTrainings = trainings.OrderBy(t => t.StartTime).ToList();
            double totalDaysBetweenSessions = 0;

            for (int i = 1; i < orderedTrainings.Count; i++)
            {
                totalDaysBetweenSessions += (orderedTrainings[i-1].StartTime - orderedTrainings[i].EndTime)?.TotalDays ?? 0;
            }

            return totalDaysBetweenSessions / (trainings.Count - 1);
        }
        public async Task UpdateTrainingAsync(int trainingId, TrainingPointDto newPointDto)
        {
            var training =  _context.Trainings.Where(x => x.Id == trainingId).Include(t => t.TrainingPoints)
                .FirstOrDefault();

            if (training == null) return;
            var newPoint = new TrainingPoint
            {
                Latitude = newPointDto.Latitude,
                Longitude = newPointDto.Longitude,
                PointTime = newPointDto.PointTime,
                TrainingId = trainingId
            };
            // Add the new training point
            _context.TrainingPoints.Add(newPoint);
            var allPoints = _context.TrainingPoints.Where(x => x.TrainingId == trainingId).ToList();

            // Calculate the speed between the last point and the new point
            if (allPoints.Count > 1)
            {
                var lastPoint = allPoints.OrderByDescending(tp => tp.PointTime).Skip(1).First();
                var speed = CalculateSpeedBetweenPoints(lastPoint, newPoint);
                training.TopSpeed = training.TopSpeed == null ? speed : Math.Max((double)training.TopSpeed, speed); // Update the top speed if the new speed is higher
                var additionalDistance = CalculateDistanceBetweenPoints(lastPoint, newPoint);
                training.DistanceCovered += additionalDistance;
            }

            await _context.SaveChangesAsync();
        }
        private double CalculateSpeedBetweenPoints(TrainingPoint p1, TrainingPoint p2)
        {
            var distance = CalculateDistanceBetweenPoints(p1, p2);
            var timeDiff = Math.Abs((p2.PointTime - p1.PointTime).TotalHours);
            return timeDiff > 0 ? distance / timeDiff : 0; // Speed in km/h
        }
    }
}
