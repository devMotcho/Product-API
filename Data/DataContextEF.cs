using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;
        public DataContextEF(IConfiguration config)
        {
            _config = config;
        }

        public virtual DbSet<Product> Products {set; get;}
        public virtual DbSet<User> Users {set; get;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    _config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure()
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("MyAppSchema");

            modelBuilder.Entity<Product>()
                .ToTable("Products", "MyAppSchema")
                .HasKey(p => p.ProductId);
            
            modelBuilder.Entity<User>()
                .ToTable("Users", "MyAppSchema")
                .HasKey(u => u.UserId);


        }
    }
}