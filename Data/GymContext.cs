using Microsoft.EntityFrameworkCore;
using GymSystem.Models;

namespace GymSystem.Data
{
    public class GymContext : DbContext
    {
        public GymContext(DbContextOptions<GymContext> options) : base(options) { }

        public DbSet<Client> client { get; set; }
        public DbSet<Membership> membership { get; set;}

        public DbSet<Activity> activity { get; set; }

        public DbSet<Payment> payment { get; set; }

        public DbSet<Trainer> trainer { get; set; }

        public DbSet<ActivitySchedule> activityschedule { get; set; }

        public DbSet<TrainerActivity> trainer_activity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. TrainerActivity – Composite Key
            modelBuilder.Entity<TrainerActivity>()
                .HasKey(ta => new { ta.activity_id, ta.trainer_id });

            modelBuilder.Entity<TrainerActivity>()
                .HasOne(ta => ta.Activity)
                .WithMany(a => a.TrainerActivities)
                .HasForeignKey(ta => ta.activity_id);

            modelBuilder.Entity<TrainerActivity>()
                .HasOne(ta => ta.Trainer)
                .WithMany(t => t.TrainerActivities)
                .HasForeignKey(ta => ta.trainer_id);

            // 2. ActivitySchedule – Foreign Key to Activity
            modelBuilder.Entity<ActivitySchedule>()
                .HasKey(s => s.schedule_id);

            modelBuilder.Entity<ActivitySchedule>()
                .HasOne(s => s.Activity)
                .WithMany(a => a.Schedules)
                .HasForeignKey(s => s.activity_id);

            // 3. Payment – Foreign Key to Membership
            modelBuilder.Entity<Payment>()
                .HasKey(p => p.payment_id);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Membership)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.membership_id);

            // 4. Membership – Foreign Keys to Client and Activity
            modelBuilder.Entity<Membership>()
                .HasKey(m => m.membership_id);

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.Client)
                .WithMany(c => c.Memberships)
                .HasForeignKey(m => m.client_id);

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.Activity)
                .WithMany(a => a.Memberships)
                .HasForeignKey(m => m.activity_id);

            // 5. Client – Primary Key
            modelBuilder.Entity<Client>()
                .HasKey(c => c.client_id);

            // 6. Activity – Primary Key
            modelBuilder.Entity<Activity>()
                .HasKey(a => a.activity_id);

            // 7. Trainer – Primary Key
            modelBuilder.Entity<Trainer>()
                .HasKey(t => t.trainer_id);
        }


    }



}