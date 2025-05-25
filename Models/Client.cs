using System.ComponentModel.DataAnnotations;

namespace GymSystem.Models
{
    public class Client

    {
        [Key]
        public int client_id { get; set; }
        [Required]
        public string client_fname { get; set; }
        [Required]
        public string client_lname { get; set; }
        [Required]
        public string address { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string phone { get; set; }

        public List<Membership> Memberships { get; set; } = new List<Membership>();
    }
}
