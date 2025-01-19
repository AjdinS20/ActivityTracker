using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ActivityTracker.Models
{
    public class TrainingPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Automatically generate ID
        public int Id { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public DateTime PointTime { get; set; } // Exact time of the point

        // Foreign key for Training
        [Required]
        public int TrainingId { get; set; }

        [ForeignKey("TrainingId")]
        public Training Training { get; set; }
    }
}
