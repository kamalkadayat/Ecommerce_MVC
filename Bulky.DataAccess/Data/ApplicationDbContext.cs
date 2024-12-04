using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAcess.Data
{
    public class ApplicationDbContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        { 

        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayName = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayName = 2 },
                new Category { Id = 3, Name = "History", DisplayName = 3 }
                );
        }
    }
}
