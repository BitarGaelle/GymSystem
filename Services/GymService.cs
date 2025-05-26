using GymSystem.Data;
using GymSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace GymSystem.Services
{
    public interface IGymService
    {
        Task<Dictionary<string, int>> GetMembershipPricesAsync();
        Task<int> AddMembershipAsync(AddMembershipDto dto, int totalPrice, string paymentMethod);
        Task<List<MembershipDetailsDto>> GetMembershipByEmailAsync(string email);
        Task<List<ActivityScheduleDto>> GetActivityScheduleAsync();
        Task<TotalCountDto> GetTotalCountAsync();
        Task<List<Client>> GetAllClientsAsync();
        Task<List<Membership>> GetAllMembershipsAsync();
        Task<bool> DeleteMembershipAsync(int membershipId);
        Task<List<Activity>> GetAllActivitiesAsync();
        Task<List<Trainer>> GetAllTrainersAsync();
        Task<int> AddActivityAsync(AddActivityDto dto);
        Task<bool> DeleteActivityAsync(int activityId);
    }

    public class GymService : IGymService
    {
        private readonly GymContext _context;

        public GymService(GymContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, int>> GetMembershipPricesAsync()
        {
            var activities = await _context.activity
                .Select(a => new { a.activity_name, a.act_price })
                .ToListAsync();

            return activities.ToDictionary(a => a.activity_name, a => a.act_price);
        }

        public async Task<int> AddMembershipAsync(AddMembershipDto dto, int totalPrice, string paymentMethod)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if client exists
                var client = await _context.client
                    .FirstOrDefaultAsync(c => c.email == dto.Email);

                if (client == null)
                {
                    // Create new client
                    client = new Client
                    {
                        client_fname = dto.ClientFname,
                        client_lname = dto.ClientLname,
                        address = dto.Address,
                        email = dto.Email,
                        phone = dto.Phone
                    };
                    _context.client.Add(client);
                    await _context.SaveChangesAsync();
                }

                // Get activity
                var activity = await _context.activity
                    .FirstOrDefaultAsync(a => a.activity_name == dto.MembershipType);

                if (activity == null)
                    throw new InvalidOperationException("Activity not found");

                DateTime dtoStartDate = DateTime.Parse(dto.StartDate);
                DateTime dtoEndDate = DateTime.Parse(dto.EndDate);

                // Check for existing membership conflict
                var existingMemberships = await _context.membership
                    .Where(m => m.client_id == client.client_id &&
                                m.activity_id == activity.activity_id)
                    .ToListAsync();

                // Convert strings to DateTime in memory and check overlap
                var conflictingMembership = existingMemberships
                    .FirstOrDefault(m =>
                    {
                        var existingStart = DateTime.Parse(m.start_date);
                        var existingEnd = DateTime.Parse(m.end_date);

                        return (dtoStartDate >= existingStart && dtoStartDate <= existingEnd) ||
                               (dtoEndDate >= existingStart && dtoEndDate <= existingEnd) ||
                               (dtoStartDate <= existingStart && dtoEndDate >= existingEnd);
                    });

                if (conflictingMembership != null)
                    throw new InvalidOperationException("Client already enrolled in this activity during the specified time range.");

                // Create membership
                var membership = new Membership
                {
                    membership_type = dto.MembershipType,
                    price = totalPrice.ToString(),
                    start_date = dto.StartDate,
                    end_date = dto.EndDate,
                    client_id = client.client_id,
                    activity_id = activity.activity_id
                };

                _context.membership.Add(membership);
                await _context.SaveChangesAsync();

                // Create payment
                var payment = new Payment
                {
                    payment_method = paymentMethod,
                    amount = decimal.Parse(totalPrice.ToString()),
                    payment_date = DateTime.Now,
                    membership_id = membership.membership_id
                };

                _context.payment.Add(payment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return membership.membership_id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<MembershipDetailsDto>> GetMembershipByEmailAsync(string email)
        {
            var memberships = await _context.membership
                .Include(m => m.Client)
                .Include(m => m.Activity)
                .Include(m => m.Payments)
                .Where(m => m.Client.email == email)
                .Select(m => new MembershipDetailsDto
                {
                    ClientFname = m.Client.client_fname,
                    ClientLname = m.Client.client_lname,
                    Email = m.Client.email,
                    Phone = m.Client.phone,
                    Address = m.Client.address,
                    MembershipType = m.membership_type,
                    StartDate = m.start_date.ToString(),
                    EndDate = m.end_date.ToString(),
                    ActivityName = m.Activity.activity_name,
                    PaymentMethod = m.Payments.First().payment_method,
                    Amount = m.Payments.First().amount,
                    PaymentDate = m.Payments.First().payment_date
                })
                .ToListAsync();

            return memberships;
        }

        public async Task<List<ActivityScheduleDto>> GetActivityScheduleAsync()
        {
            var schedules = await _context.activityschedule
                .Include(s => s.Activity)
                .Select(s => new ActivityScheduleDto
                {
                    ActivityId = s.activity_id,
                    ActivityName = s.Activity.activity_name,
                    DayOfWeek = s.day_of_week,
                    StartHour = s.start_hour,
                    EndHour = s.end_hour
                })
                .ToListAsync();

            return schedules;
        }

        public async Task<TotalCountDto> GetTotalCountAsync()
        {
            var totalClients = await _context.client.CountAsync();
            var totalMemberships = await _context.membership.CountAsync();
            var totalActivities = await _context.activity.CountAsync();

            return new TotalCountDto
            {
                TotalClients = totalClients,
                TotalMemberships = totalMemberships,
                TotalActivities = totalActivities
            };
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            return await _context.client.ToListAsync();
        }

        public async Task<List<Membership>> GetAllMembershipsAsync()
        {
            return await _context.membership
                .Include(m => m.Client)
                .Include(m => m.Activity)
                .ToListAsync();
        }

        public async Task<bool> DeleteMembershipAsync(int membershipId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete payments first
                var payments = await _context.payment
                    .Where(p => p.membership_id == membershipId)
                    .ToListAsync();

                _context.payment.RemoveRange(payments);

                // Delete membership
                var membership = await _context.membership
                    .FindAsync(membershipId);

                if (membership == null)
                    return false;

                _context.membership.Remove(membership);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Activity>> GetAllActivitiesAsync()
        {
            return await _context.activity.ToListAsync();
        }

        public async Task<List<Trainer>> GetAllTrainersAsync()
        {
            return await _context.trainer.ToListAsync();
        }

        public async Task<int> AddActivityAsync(AddActivityDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create activity
                var activity = new Activity
                {
                    activity_name = dto.ActivityName,
                    act_price = dto.ActPrice
                };

                _context.activity.Add(activity);
                await _context.SaveChangesAsync();

                // Add schedules
                foreach (var schedule in dto.Schedules)
                {
                    var activitySchedule = new ActivitySchedule
                    {
                        activity_id = activity.activity_id,
                        day_of_week = schedule.Day,
                        start_hour = schedule.StartTime,
                        end_hour = schedule.EndTime
                    };
                    _context.activityschedule.Add(activitySchedule);
                }

                // Link trainer
                var trainerActivity = new TrainerActivity
                {
                    activity_id = activity.activity_id,
                    trainer_id = dto.TrainerId
                };
                _context.trainer_activity.Add(trainerActivity);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return activity.activity_id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteActivityAsync(int activityId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete trainer activities
                var trainerActivities = await _context.trainer_activity
                    .Where(ta => ta.activity_id == activityId)
                    .ToListAsync();
                _context.trainer_activity.RemoveRange(trainerActivities);

                // Delete activity schedules
                var schedules = await _context.activityschedule
                    .Where(s => s.activity_id == activityId)
                    .ToListAsync();
                _context.activityschedule.RemoveRange(schedules);

                // Delete activity
                var activity = await _context.activity.FindAsync(activityId);
                if (activity == null)
                    return false;

                _context.activity.Remove(activity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}