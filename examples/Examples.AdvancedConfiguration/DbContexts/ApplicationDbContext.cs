using Microsoft.EntityFrameworkCore;

namespace Examples.AdvancedConfiguration.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}