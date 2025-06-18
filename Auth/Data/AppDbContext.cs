using Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        public DbSet<User> Users { get; set; } 
        
    }
   
}
