using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chushka.Data;
using Microsoft.AspNetCore.Mvc;
using Chushka.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Chushka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChushkaDbContext _dbContext;

        public HomeController(ChushkaDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var products = await this._dbContext
                                .Products
                                .Select(x => new ShowProductViewModel(x.Id, x.Name, x.Price, x.Description, x.Type.ToString()))
                                .ToListAsync();

                return View("IndexLoggedIn", products);
            }
            return View();
        }
    }
}
