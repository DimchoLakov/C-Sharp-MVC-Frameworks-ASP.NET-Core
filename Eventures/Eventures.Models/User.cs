using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Eventures.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            this.Orders = new HashSet<Order>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UniqueCitizenNumber { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
