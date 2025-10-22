using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Models;

namespace PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ClaimAttachment> ClaimAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Claim ↔ User relationship
            modelBuilder.Entity<Claim>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // ClaimAttachment ↔ Claim relationship
            modelBuilder.Entity<ClaimAttachment>()
                .HasOne<Claim>()
                .WithMany()
                .HasForeignKey(ca => ca.ClaimID)
                .OnDelete(DeleteBehavior.Cascade);

            // Review ↔ Claim relationship
            modelBuilder.Entity<Review>()
                .HasOne<Claim>()
                .WithMany()
                .HasForeignKey(r => r.ClaimID)
                .OnDelete(DeleteBehavior.Cascade);

            // Review ↔ User (Reviewer) relationship
            modelBuilder.Entity<Review>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
