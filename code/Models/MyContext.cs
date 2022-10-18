using Microsoft.EntityFrameworkCore;

namespace AngularClient.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<Customer>? Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(new Customer
            {
                Id = 1,
                CustomerName = "Trailhead Technology Partners"
            }, new Customer
            {
                Id = 2,
                CustomerName = "Microsoft"
            });

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                UserName = "user",
                Password = "password"
            });
        }
    }
}
