using System.ComponentModel.DataAnnotations;


namespace GymSystem.Models
{
    public class TrainerActivity

    {
        
        [Required]
        public int activity_id { get; set; }

        public Activity Activity { get; set; }

        
        [Required]
        public int trainer_id { get; set; }
        public Trainer Trainer { get; set; }
    }
}
