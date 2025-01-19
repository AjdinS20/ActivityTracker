using ActivityTracker.Models.Request_Models;

namespace ActivityTracker.Models.Response_Models
{
    public class TrainingDetailDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TrainingType Type { get; set; }
        public double TotalTime { get; set; } // In seconds
        public double? TopSpeed { get; set; } // In km/h
        public double AverageSpeed { get; set; } // In km/h
        public double DistanceCovered { get; set; } // In km
        public double CaloriesBurned { get; set; } // To be calculated later
        public List<TrainingPointDto> Points { get; set; }
    }
}
