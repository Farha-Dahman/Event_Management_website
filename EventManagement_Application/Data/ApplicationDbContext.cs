using EventManagement_Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Reflection.Emit;

namespace EventManagement_Application.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<OrganizerRequest> OrganizerRequests { get; set; }
        public DbSet<TicketUser> TicketUsers { get; set; }
        public DbSet<EventViewer> EventViewers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<ContactForm> ContactForms { get; set; }
        public DbSet<FavoriteEvent> Favorites { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users", "security");
            builder.Entity<IdentityRole>().ToTable("Roles", "security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "security");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "security");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "security");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "security");

            // Define composite primary key
            builder.Entity<TicketUser>()
                .HasKey(tu => new { tu.TicketId, tu.UserId });

            // Define relationship with Ticket
            builder.Entity<TicketUser>()
                .HasOne(tu => tu.Ticket)
                .WithMany(t => t.TicketUsers)
                .HasForeignKey(tu => tu.TicketId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete

            // Define relationship with ApplicationUser
            builder.Entity<TicketUser>()
                .HasOne(tu => tu.User)
                .WithMany(u => u.TicketUsers)
                .HasForeignKey(tu => tu.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<EventViewer>()
                .HasOne(ev => ev.Event)
                .WithMany()
                .HasForeignKey(ev => ev.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EventViewer>()
                .HasOne(ev => ev.Viewer)
                .WithMany()
                .HasForeignKey(ev => ev.ViewerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FavoriteEvent>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FavoriteEvent>()
                .HasOne(ev => ev.Event)
                .WithMany()
                .HasForeignKey(ev => ev.EventId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
