using System;
using System.Threading.Tasks;
using Chushka.Data;
using Chushka.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chushka.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ChushkaDbContext _dbContext;

        public OrdersController(ChushkaDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> All()
        {
            var orders = await this._dbContext.Orders.ToListAsync();

            return this.View(orders);
        }

        [Authorize]
        public async Task<IActionResult> Create(int id)
        {
            var username = this.User.Identity.Name;
            var user = await this._dbContext
                .Users
                .FirstOrDefaultAsync(x => x.UserName == username);
            var product = await this._dbContext
                .Products
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null || product == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            var order = new Order()
            {
                ClientId = user.Id,
                Client = user,
                Product = product,
                ProductId = id,
                OrderedOn = DateTime.UtcNow
            };

            await this._dbContext
                .Orders
                .AddAsync(order);

            await this._dbContext
                .SaveChangesAsync();

            if (this.User.IsInRole("Admin"))
            {
                return this.RedirectToAction("All", "Orders");
            }

            return this.RedirectToAction("Index", "Home");
        }
    }
}
