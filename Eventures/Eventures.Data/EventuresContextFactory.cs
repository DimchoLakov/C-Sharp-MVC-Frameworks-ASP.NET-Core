using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Eventures.Data
{
    public class EventuresContextFactory : IDesignTimeDbContextFactory<EventuresDbContext>
    {
        public EventuresDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EventuresDbContext>();

            optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=Eventures;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new EventuresDbContext(optionsBuilder.Options);
        }
    }
}
