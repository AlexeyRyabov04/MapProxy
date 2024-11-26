using MapProxy.Models;
using Microsoft.EntityFrameworkCore;

namespace MapProxy.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<AccessRule> AccessRules { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

    }
}
