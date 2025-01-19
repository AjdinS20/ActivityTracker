using System.ComponentModel.DataAnnotations;

namespace ActivityTracker.Models.Request_Models
{
    public class TrainingPointDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime PointTime { get; set; }
    }
}
