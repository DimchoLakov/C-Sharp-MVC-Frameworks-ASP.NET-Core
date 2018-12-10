using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Eventures.Data;
using Eventures.Models;
using Eventures.Web.Filters;
using Eventures.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Eventures.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventuresDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<EventsController> _logger;

        public EventsController(EventuresDbContext dbContext, IMapper mapper, ILogger<EventsController> logger)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
            this._logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> All(int? currentPage = 1)
        {
            var events = await this._dbContext.Events
                                              .Where(x => x.TotalTickets > 0)
                                              .ToListAsync();
            var allEvents = this._mapper.Map<List<Event>, IEnumerable<CreateEventViewModel>>(events);
            var count = events.Count();
            var size = 10;
            var totalPages = (int)Math.Ceiling(decimal.Divide(count, size));

            if (currentPage <= 1)
            {
                currentPage = 1;
            }
            if (currentPage >= totalPages)
            {
                currentPage = totalPages;
            }
            
            var skip = (int)(currentPage - 1) * size;
            var take = size;
           
            allEvents = allEvents.Skip(skip).Take(take).ToList();

            var viewModel = new CreateEventOrderViewModel()
            {
                CreateEventViewModels = allEvents,
                CreateOrderViewModel = new CreateOrderViewModel(),
                CurrentPage = (int)currentPage,
                FirstPage = 1,
                LastPage = totalPages
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(EventsLogActionFilter))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateEventViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Preventing creating invalid Id for Event
                viewModel.Id = null;

                var ev = this._mapper.Map<CreateEventViewModel, Event>(viewModel);
                await this._dbContext
                    .Events
                    .AddAsync(ev);

                await this._dbContext
                    .SaveChangesAsync();

                this._logger.LogInformation("Event created: ", ev.Name, ev);

                return RedirectToAction("All");
            }

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> MyEvents()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var currentUserEvents = await this._dbContext
                .Users
                .Where(x => x.Id == userId)
                .SelectMany(x => x.Orders).Select(x => x.Event)
                .Select(x => new MyEventViewModel()
                {
                    Name = x.Name,
                    Start = x.Start,
                    End = x.End,
                    Tickets = x.Orders
                        .Where(o => o.CustomerId == userId)
                        .Sum(t => t.TicketsCount)
                })
                .Distinct()
                .ToListAsync();

            return View(currentUserEvents);
        }
    }
}