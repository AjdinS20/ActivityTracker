namespace ActivityTracker.Models.Response_Models
{
    public class TrainingStatsDto
    {
        public int NumberOfTrainings { get; set; }
        public double? TotalTimeSpent { get; set; } // In hours
        public double? TotalDistanceCovered { get; set; } // In km
        public double? TotalCaloriesBurned { get; set; }
        public double? TopSpeed { get; set; } // In km/h
        public TimeSpan? LongestSession { get; set; }
        public double? LongestRouteCovered { get; set; } // In km
        public double? AvgTimeBetweenSessions { get; set; } // In days
    }
}
