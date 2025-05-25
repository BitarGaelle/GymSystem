using System.ComponentModel.DataAnnotations;

namespace GymSystem.Models
{
    public class Activity
    {
        [Key]
        public int activity_id { get; set; }
        [Required]
        public string activity_name { get; set; }
        [Required]
        public int act_price { get; set; }

        public List<Membership> Memberships { get; set; } = new List<Membership>();
        public List<ActivitySchedule> Schedules { get; set; } = new List<ActivitySchedule>();
        public List<TrainerActivity> TrainerActivities { get; set; } = new List<TrainerActivity>();
    }
}
