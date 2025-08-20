using FigureGear.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace FigureGear.Data.Context
{
    public class FigureGearDbContext : DbContext
    {
        public FigureGearDbContext()
        {
        }

        public FigureGearDbContext(DbContextOptions<FigureGearDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.UserName)
                       .HasMaxLength(50)
                       .IsRequired();

                builder.Property(x => x.Email)
                       .HasMaxLength(100)
                       .IsRequired();

                builder.Property(x => x.PasswordHash)
                       .HasMaxLength(256)
                       .IsRequired();

                builder.Property(x => x.SecurityStamp)
                       .HasMaxLength(100)
                       .IsRequired();

                builder.Property(x => x.IsActive)
                       .IsRequired();

                builder.Property(x => x.IsDeleted)
                       .IsRequired();

                builder.Property(x => x.EmailConfirmed)
                       .IsRequired();

                builder.Property(x => x.FailedLoginCount)
                       .IsRequired();

                builder.Property(x => x.LockoutEnd)
                       .IsRequired(false);

                builder.Property(x => x.CreatedDate)
                       .IsRequired();

                builder.Property(x => x.LastLoginDate)
                       .IsRequired(false);

                builder.Property(x => x.UpdatedDate)
                       .IsRequired(false);

                builder.Property(x => x.RefreshToken)
                       .HasMaxLength(512)
                       .IsRequired(false);

                builder.Property(x => x.RefreshTokenExpiryTime)
                       .IsRequired(false);

                builder.HasIndex(x => x.UserName).IsUnique();

                builder.HasIndex(x => x.Email).IsUnique();
            });

            modelBuilder.Entity<UserRole>(builder =>
            {
                builder.HasKey(ur => new { ur.UserId, ur.RoleId });

                builder.HasOne(ur => ur.User)
                       .WithMany(u => u.UserRoles)
                       .HasForeignKey(ur => ur.UserId);

                builder.HasOne(ur => ur.Role)
                       .WithMany(r => r.UserRoles)
                       .HasForeignKey(ur => ur.RoleId);
            });

        }
    }
}
