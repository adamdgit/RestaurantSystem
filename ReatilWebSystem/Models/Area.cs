using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using BitByByte.Data;
using Microsoft.AspNetCore.Identity;

namespace BitByByte.Models
{

    public class Area
    {
        [Key]
        public int AreaId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class SeedData
    {
        public static async Task Initialize(UserManager<ApplicationUser> userManager, RestaurantDbContext context)
        {
            if (!context.Users.Any()) 
            {
                var Admin = new ApplicationUser
                {
                    Role = "admin",
                    FirstName = "Test",
                    LastName = "Admin",
                    Email = "admin@beanscene.com",
                    UserName = "admin@beanscene.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(Admin, "Password123*");
                await userManager.AddToRoleAsync(Admin, "admin");

                var Staff = new ApplicationUser
                {
                    Role = "staff",
                    FirstName = "Test",
                    LastName = "Staff",
                    Email = "staff@beanscene.com",
                    UserName = "staff@beanscene.com",
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(Staff, "Password123*");
                await userManager.AddToRoleAsync(Staff, "staff");
            }

            if (!context.Area.Any())
            {
                context.Area.AddRange(
                    new Area { AreaId = 1, Name = "Main", Description = "The primary dining area with a cozy ambiance." },
                    new Area { AreaId = 2, Name = "Outdoor", Description = "A delightful outdoor seating area, surrounded by lush trees." },
                    new Area { AreaId = 3, Name = "Balcony", Description = "An elevated balcony space with a scenic outlook." }
                );
            }

            if (!context.TableInfo.Any())
            {
                context.TableInfo.AddRange(
                    new TableInfo { Name = "Main 1", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { Name = "Main 2", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { Name = "Main 3", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { Name = "Main 4", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { Name = "Main 5", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { Name = "Main 6", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { Name = "Main 7", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { Name = "Main 8", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { Name = "Main 9", Seats = 2, Availability = "Available", AreaId = 1 },
                    new TableInfo { Name = "Main 10", Seats = 2, Availability = "Available", AreaId = 1 },

                    new TableInfo { Name = "Outdoor 1", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { Name = "Outdoor 2", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { Name = "Outdoor 3", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { Name = "Outdoor 4", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { Name = "Outdoor 5", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { Name = "Outdoor 6", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { Name = "Outdoor 7", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { Name = "Outdoor 8", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { Name = "Outdoor 9", Seats = 2, Availability = "Available", AreaId = 2 },
                    new TableInfo { Name = "Outdoor 10", Seats = 2, Availability = "Available", AreaId = 2 },

                    new TableInfo { Name = "Balcony 1", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { Name = "Balcony 2", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { Name = "Balcony 3", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { Name = "Balcony 4", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { Name = "Balcony 5", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { Name = "Balcony 6", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { Name = "Balcony 7", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { Name = "Balcony 8", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { Name = "Balcony 9", Seats = 2, Availability = "Available", AreaId = 3 },
                    new TableInfo { Name = "Balcony 10", Seats = 2, Availability = "Available", AreaId = 3 }
                );
            }
            context.SaveChanges();
        }
    }
}