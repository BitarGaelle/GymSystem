using System.ComponentModel.DataAnnotations;


namespace GymSystem.Models
{
    public class Trainer
    {
        [Key]
        public int trainer_id { get; set; }
        [Required]
        public string trainer_name { get; set; }

        public string trainer_phone { get; set; }

        public List<TrainerActivity> TrainerActivities { get; set; } = new List<TrainerActivity>();
    }

}
