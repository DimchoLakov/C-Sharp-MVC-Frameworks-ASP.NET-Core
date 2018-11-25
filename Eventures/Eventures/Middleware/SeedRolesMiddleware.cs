using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Eventures.Data;
using Eventures.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Eventures.Web.Middleware
{
    public class SeedRolesMiddleware
    {
        private readonly RequestDelegate _next;

        public SeedRolesMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var dbContext = serviceProvider.GetService<EventuresDbContext>();

            if (!dbContext.Roles.Any())
            {
                await this.SeedRoles(userManager, roleManager);
            }

            await this._next(context);
        }

        private async Task SeedRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
    }
}
