using System.ComponentModel.DataAnnotations;


namespace GymSystem.Models
{

    public class ActivitySchedule
    {
        [Key]
        public int schedule_id { get; set; }
        public int activity_id { get; set; }
        public Activity Activity { get; set; }
        [Required]
        public string day_of_week { get; set; }
        [Required]
        public string start_hour { get; set; }
        [Required]
        public string end_hour { get; set; }
    }
}
