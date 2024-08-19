using Microsoft.EntityFrameworkCore;
using BitByByte.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BitByByte.Data;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;

namespace BitByByte.Data
{
    public class RestaurantDbContext : IdentityDbContext<ApplicationUser>
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
        : base(options)
        {

        }

        public DbSet<Area> Area { get; set; } = default!;
        public DbSet<AssignedTable> AssignedTable { get; set; } = default!;
        public DbSet<Reservation> Reservation { get; set; } = default!;
        public DbSet<Sitting> Sitting { get; set; } = default!;
        public DbSet<TableInfo> TableInfo { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Area>().HasData(
                    new Area { AreaId = 1, Name = "Main", Description = "The primary dining area with a cozy ambiance." },
                    new Area { AreaId = 2, Name = "Outdoor", Description = "A delightful outdoor seating area, surrounded by lush trees." },
                    new Area { AreaId = 3, Name = "Balcony", Description = "An elevated balcony space with a scenic outlook." }
            );

            modelBuilder.Entity<TableInfo>().HasData(
                    new TableInfo { TableId = 1, Name = "Main 1", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { TableId = 2, Name = "Main 2", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { TableId = 3, Name = "Main 3", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { TableId = 4, Name = "Main 4", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { TableId = 5, Name = "Main 5", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { TableId = 6, Name = "Main 6", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { TableId = 7, Name = "Main 7", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { TableId = 8, Name = "Main 8", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { TableId = 9, Name = "Main 9", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { TableId = 10, Name = "Main 10", Seats = 2, Availability = "Available", AreaId = 1 },

                    new TableInfo { TableId = 11, Name = "Outdoor 1", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { TableId = 12, Name = "Outdoor 2", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { TableId = 13, Name = "Outdoor 3", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { TableId = 14, Name = "Outdoor 4", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { TableId = 15, Name = "Outdoor 5", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { TableId = 16, Name = "Outdoor 6", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { TableId = 17, Name = "Outdoor 7", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { TableId = 18, Name = "Outdoor 8", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { TableId = 19, Name = "Outdoor 9", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { TableId = 20, Name = "Outdoor 10", Seats = 2, Availability = "Available", AreaId = 2 },

                    new TableInfo { TableId = 21, Name = "Balcony 1", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { TableId = 22, Name = "Balcony 2", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { TableId = 23, Name = "Balcony 3", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { TableId = 24, Name = "Balcony 4", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { TableId = 25, Name = "Balcony 5", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { TableId = 26, Name = "Balcony 6", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { TableId = 27, Name = "Balcony 7", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { TableId = 28, Name = "Balcony 8", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { TableId = 29, Name = "Balcony 9", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { TableId = 30, Name = "Balcony 10", Seats = 2, Availability = "Available", AreaId = 3 }
            );

            var admin = new ApplicationUser
            {
                Role = "admin",
                FirstName = "Test",
                LastName = "Admin",
                Email = "admin@beanscene.com",
                NormalizedEmail = "ADMIN@BEANSCENE.COM",
                UserName = "admin@beanscene.com",
                NormalizedUserName = "ADMIN@BEANSCENE.COM",
                EmailConfirmed = true,
                PhoneNumber = "12345678",
                ProfileUrl = "none",
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            admin.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(admin, "Password123*");

            var staff = new ApplicationUser
            {
                Role = "staff",
                FirstName = "Test",
                LastName = "Staff",
                Email = "staff@beanscene.com",
                NormalizedEmail = "STAFF@BEANSCENE.COM",
                UserName = "staff@beanscene.com",
                NormalizedUserName = "STAFF@BEANSCENE.COM",
                EmailConfirmed = true,
                PhoneNumber = "12345679",
                ProfileUrl = "none",
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            staff.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(staff, "Password123*");

            modelBuilder.Entity<ApplicationUser>().HasData(
                admin, 
                staff
            );

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "staff", NormalizedName = "STAFF" },
                new IdentityRole { Id = "3", Name = "user", NormalizedName = "USER" }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = admin.Id, RoleId = "1" },
                new IdentityUserRole<string> { UserId = staff.Id, RoleId = "2" }
            );


        }
    }
}
