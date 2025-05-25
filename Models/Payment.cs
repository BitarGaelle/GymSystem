using System.ComponentModel.DataAnnotations;


namespace GymSystem.Models
{
    public class Payment
    {
        [Key]
        public int payment_id { get; set; }
        [Required]
        public string payment_method { get; set; }
        [Required]
        public decimal amount { get; set; }
        public DateTime payment_date { get; set; }

        public int membership_id { get; set; }
        public Membership Membership { get; set; }
    }
}
