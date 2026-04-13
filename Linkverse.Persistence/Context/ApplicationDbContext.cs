using Linkverse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Linkverse.Persistence.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; } 
    }
}
