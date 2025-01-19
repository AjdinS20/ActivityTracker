using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ActivityTracker.Models
{
    public class Training
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Automatically generate ID
        public int Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        public TrainingType Type { get; set; }

        // Navigation property for TrainingPoints
        public ICollection<TrainingPoint> TrainingPoints { get; set; }

        public double? DistanceCovered { get; set; } // In kilometers (km)

        public double? CaloriesBurned { get; set; }

        public double? TopSpeed { get; set; } // In km/h

        // Foreign key for ApplicationUser
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }

    public enum TrainingType
    {
        Running = 0,
        Cycling = 1
    }
}
