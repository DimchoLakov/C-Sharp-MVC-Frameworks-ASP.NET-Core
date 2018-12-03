using System;
using System.Collections.Generic;
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
                var currentEvent = await this._dbContext.Events
                                                        .FirstOrDefaultAsync(x => x.Id == viewModel.EventId);

                if (viewModel.TicketsCount > currentEvent.TotalTickets)
                {
                    ModelState.AddModelError(string.Empty, "Event does not have enough tickets!");
                    return RedirectToAction("All", "Events");
                }

                //viewModel.TicketsCount = Math.Max(0,
                //    currentEvent.TotalTickets - (currentEvent.TotalTickets - viewModel.TicketsCount));

                currentEvent.TotalTickets -= viewModel.TicketsCount;

                this._dbContext.Events.Update(currentEvent);

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
            var orders = await this._dbContext
                                        .Orders
                                        .ToListAsync();

            var allOrders = this._mapper.Map<List<Order>, IEnumerable<OrderViewModel>>(orders);
            allOrders = allOrders.OrderByDescending(x => x.OrderedOn);
            return View(allOrders);
        }
    }
}