using Core_3._1.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core_3._1.Models
{
    public class AppDBContext : IdentityDbContext<ApplicationUser>
    {
        private readonly DbContextOptions _options;

        public AppDBContext(DbContextOptions options): base(options)
        {
            _options = options; 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
