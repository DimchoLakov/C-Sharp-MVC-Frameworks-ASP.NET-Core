using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Eventures.Data;
using Eventures.Models;
using Eventures.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventures.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly EventuresDbContext _dbContext;
        private readonly IMapper _mapper;

        public OrdersController(EventuresDbContext dbContext, IMapper mapper)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateOrderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var order = this._mapper.Map<CreateOrderViewModel, Order>(viewModel);

                await this._dbContext
                    .Orders
                    .AddAsync(order);

                await this._dbContext
                    .SaveChangesAsync();

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("All", "Orders");
                }

                return RedirectToAction("MyEvents", "Events");
            }

            ModelState.AddModelError(string.Empty, "Try placing your order again.");
            return RedirectToAction("All", "Events");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> All()
        {
            var allOrders = await this._dbContext
                .Orders
                .Select(o => new OrderViewModel()
                {
                    EventName = o.Event.Name,
                    CustomerName = o.Customer.UserName,
                    OrderedOn = DateTime.UtcNow
                })
                .ToListAsync();

            return View(allOrders);
        }
    }
}