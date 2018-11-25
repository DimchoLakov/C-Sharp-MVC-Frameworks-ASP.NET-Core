using Eventures.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Eventures.Data
{
    public class EventuresDbContext : IdentityDbContext<User>
    {
        public EventuresDbContext(DbContextOptions<EventuresDbContext> options) : base(options)
        {
        }
        
        public virtual DbSet<Event> Events { get; set; }
    }
}
