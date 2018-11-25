using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Eventures.Data;
using Eventures.Models;
using Eventures.Web.Filters;
using Eventures.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult All()
        {
            var allEvents = this._dbContext
                .Events
                .Select(e => new CreateEventViewModel()
                {
                    Name = e.Name,
                    Place = e.Place,
                    PricePerTicket = e.PricePerTicket,
                    TotalTickets = e.TotalTickets,
                    Start = e.Start,
                    End = e.End
                })
                .ToList();
            
            return View(allEvents);
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
    }
}