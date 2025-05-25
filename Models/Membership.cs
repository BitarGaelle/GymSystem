using System.ComponentModel.DataAnnotations;

namespace GymSystem.Models
{
    public class Membership
    {
        [Key]
        public int membership_id { get; set; }
        [Required]
        public string membership_type { get; set; }
        [Required]
        public string price { get; set; }
        [Required]
        public string start_date { get; set; }
        [Required]
        public string end_date { get; set; }

        public int client_id { get; set; }
        public Client Client { get; set; }

        public int activity_id { get; set; }
        public Activity Activity { get; set; }

        public List<Payment> Payments { get; set; } = new List<Payment>();
    }

}
