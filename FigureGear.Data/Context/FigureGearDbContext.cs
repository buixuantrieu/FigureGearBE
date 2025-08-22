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

        public DbSet<Manufacturer> Manufacturers { get; set; }

        public DbSet<Material> Materials { get; set; }

        public DbSet<Series> Series { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<District> Districts { get; set; }

        public DbSet<Ward> Wards { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductTag> ProductTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                       .ValueGeneratedOnAdd();

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

            modelBuilder.Entity<Role>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                       .ValueGeneratedOnAdd();

                builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

                builder.Property(x => x.Description)
                .HasMaxLength(255);
            });

            modelBuilder.Entity<UserRole>(builder =>
            {
                builder.HasKey(ur => new { ur.UserId, ur.RoleId });

                builder.HasOne(ur => ur.User)
                       .WithMany(u => u.UserRoles)
                       .HasForeignKey(ur => ur.UserId)
                       .OnDelete(DeleteBehavior.Restrict); ;


                builder.HasOne(ur => ur.Role)
                       .WithMany(r => r.UserRoles)
                       .HasForeignKey(ur => ur.RoleId)
                       .OnDelete(DeleteBehavior.Restrict); ;
            });

            modelBuilder.Entity<Manufacturer>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                       .ValueGeneratedOnAdd();

                builder.Property(x => x.Name)
                       .HasMaxLength(100)
                       .IsRequired();

                builder.Property(x => x.Description)
                       .HasMaxLength(255);

                builder.Property(x => x.Country)
                       .HasMaxLength(50);

            });

            modelBuilder.Entity<Material>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                       .ValueGeneratedOnAdd();

                builder.Property(x => x.Name)
                       .HasMaxLength(100)
                       .IsRequired();

                builder.Property(x => x.Description)
                       .HasMaxLength(255);
            });

            modelBuilder.Entity<Series>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                       .ValueGeneratedOnAdd();

                builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

                builder.Property(x => x.Description)
                .HasMaxLength(255);
            });

            modelBuilder.Entity<Tag>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                       .ValueGeneratedOnAdd();

                builder.Property(x => x.Name)
                       .HasMaxLength(100);

                builder.Property(x => x.Color)
                       .HasMaxLength(10);
            });

            modelBuilder.Entity<Supplier>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                       .ValueGeneratedOnAdd();

                builder.Property(x => x.Name)
                       .IsRequired()
                       .HasMaxLength(150);

                builder.Property(x => x.AddressDetail)
                       .HasMaxLength(100)
                       .IsRequired();

                builder.Property(x => x.Phone)
                       .HasMaxLength(20)
                       .IsRequired();

                builder.Property(x => x.Email)
                       .HasMaxLength(100)
                       .IsRequired();

                builder.HasOne(x => x.City)
                       .WithMany()
                       .HasForeignKey(x => x.CityId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.District)
                       .WithMany()
                       .HasForeignKey(x => x.DistrictId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.Ward)
                       .WithMany()
                       .HasForeignKey(x => x.WardId)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Name)
                       .IsRequired()
                       .HasMaxLength(255);

                builder.Property(x => x.Description)
                       .HasColumnType("nvarchar(max)");

                builder.Property(x => x.Price)
                       .HasColumnType("decimal(18,2)")
                       .IsRequired();

                builder.Property(x => x.StockQuantity)
                       .HasDefaultValue(0);

                builder.Property(x => x.ReservedQuantity)
                       .HasDefaultValue(0);

                builder.Property(x => x.CreatedAt)
                       .HasDefaultValueSql("GETDATE()");

                builder.Property(x => x.UpdatedAt)
                       .IsRequired(false);

                builder.HasOne(x => x.Manufacturer)
                       .WithMany()
                       .HasForeignKey(x => x.ManufacturerId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.Material)
                       .WithMany()
                       .HasForeignKey(x => x.MaterialId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.Category)
                       .WithMany()
                       .HasForeignKey(x => x.CategoryId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.Series)
                       .WithMany()
                       .HasForeignKey(x => x.SeriesId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.CreatedUser)
                       .WithMany()
                       .HasForeignKey(x => x.CreatedBy)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.UpdatedUser)
                       .WithMany()
                       .HasForeignKey(x => x.UpdatedBy)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<City>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Name)
                       .IsRequired()
                       .HasMaxLength(255);

                builder.HasMany(x => x.Districts)
                       .WithOne(x => x.City)
                       .HasForeignKey(x => x.CityId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<District>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Name)
                       .IsRequired()
                       .HasMaxLength(255);

                builder.HasOne(x => x.City)
                       .WithMany(x => x.Districts)
                       .HasForeignKey(x => x.CityId);

                builder.HasMany(x => x.Wards)
                       .WithOne(x => x.District)
                       .HasForeignKey(x => x.DistrictId)
                       .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Ward>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Name)
                       .IsRequired()
                       .HasMaxLength(255);

                builder.HasOne(x => x.District)
                       .WithMany(x => x.Wards)
                       .HasForeignKey(x => x.DistrictId);
            });

            modelBuilder.Entity<ProductTag>(builder =>
            {
                builder.HasKey(pt => new { pt.ProductId, pt.TagId });

                builder.HasOne(pt => pt.Product)
                       .WithMany(p => p.ProductTags)
                       .HasForeignKey(pt => pt.ProductId);

                builder.HasOne(pt => pt.Tag)
                       .WithMany(t => t.ProductTags)
                       .HasForeignKey(pt => pt.TagId);
            });
        }
    }
}
