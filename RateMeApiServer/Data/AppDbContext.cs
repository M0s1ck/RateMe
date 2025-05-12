using Microsoft.EntityFrameworkCore;
using RateMeApiServer.Models.Entities;

namespace RateMeApiServer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ControlElement> Elements { get; set; }
    }
}
