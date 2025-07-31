using FigureGear.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace FigureGear.Data.Context
{
    public class FigureGearDbContext: DbContext
    {
        public FigureGearDbContext(DbContextOptions<FigureGearDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                        .HasIndex(u => u.UserName)
                        .IsUnique();
        }
    }
}
