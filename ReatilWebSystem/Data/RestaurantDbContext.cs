using Microsoft.EntityFrameworkCore;
using BitByByte.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
    }
}