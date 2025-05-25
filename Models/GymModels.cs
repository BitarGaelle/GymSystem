using System.ComponentModel.DataAnnotations;

namespace GymSystem.Models
{


    // DTOs for API responses
    public class MembershipDetailsDto
    {
        public string ClientFname { get; set; }
        public string ClientLname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string MembershipType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PaymentMethod { get; set; }
        public string ActivityName { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class TotalCountDto
    {
        public int TotalClients { get; set; }
        public int TotalMemberships { get; set; }
        public int TotalActivities { get; set; }
    }

    public class ActivityScheduleDto
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string DayOfWeek { get; set; }
        public string StartHour { get; set; }
        public string EndHour { get; set; }
    }

    public class AddMembershipDto
    {
        [Required]
        public string ClientFname { get; set; }
        [Required]
        public string ClientLname { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string MembershipType { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public string TotalPrice { get; set; }
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string EndDate { get; set; }
    }

    public class AddActivityDto
    {
        [Required]
        public string ActivityName { get; set; }
        [Required]
        public int ActPrice { get; set; }
        [Required]
        public int TrainerId { get; set; }
        [Required]
        public List<ScheduleDto> Schedules { get; set; } = new List<ScheduleDto>();
    }

    public class ScheduleDto
    {
        [Required]
        public string Day { get; set; }
        [Required]
        public string StartTime { get; set; }
        [Required]
        public string EndTime { get; set; }
    }
}