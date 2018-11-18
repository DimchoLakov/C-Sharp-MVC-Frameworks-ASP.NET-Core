using System.Diagnostics;
using System.Linq;
using Chushka.Data;
using Microsoft.AspNetCore.Mvc;
using Chushka.ViewModels;

namespace Chushka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChushkaDbContext _dbContext;

        public HomeController(ChushkaDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IActionResult Index()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var products = this._dbContext
                    .Products
                    .Select(x => new ShowProductViewModel(x.Id, x.Name, x.Price, x.Description, x.Type.ToString()))
                    .ToList();

                return View("IndexLoggedIn", products);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
