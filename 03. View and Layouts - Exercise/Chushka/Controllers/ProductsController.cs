using System;
using System.Linq;
using System.Threading.Tasks;
using Chushka.Data;
using Chushka.Data.Models;
using Chushka.Data.Models.Enums;
using Chushka.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chushka.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ChushkaDbContext _dbContext;

        public ProductsController(ChushkaDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await this._dbContext
                                    .Products
                                    .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel viewModel)
        {
            if (this.ModelState.IsValid)
            {
                var product = new Product()
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    Type = Enum.Parse<ProductType>(viewModel.Type)
                };

                await this._dbContext
                    .Products
                    .AddAsync(product);
                await this._dbContext
                    .SaveChangesAsync();

                return this.RedirectToAction("Index", "Home");
            }

            return this.View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await this._dbContext
                             .Products
                             .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            var viewModel = new EditViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Type = product.Type
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel viewModel)
        {
            if (this.ModelState.IsValid)
            {
                var product = await this._dbContext
                    .Products
                    .FirstOrDefaultAsync(x => x.Id == viewModel.Id);

                if (product == null)
                {
                    return this.RedirectToAction("Index", "Home");
                }

                product.Type = viewModel.Type;
                product.Description = viewModel.Description;
                product.Name = viewModel.Name;
                product.Price = viewModel.Price;

                this._dbContext.Products.Update(product);
                await this._dbContext.SaveChangesAsync();

                return this.RedirectToAction("Index", "Home");
            }

            return this.View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var product = await this._dbContext
                                    .Products
                                    .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            var viewModel = new DeleteViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Type = product.Type
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Index", "Home");
            }

            var product = await this._dbContext
                .Products
                .FirstOrDefaultAsync(x => x.Id == viewModel.Id);

            if (product == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            this._dbContext
                .Products
                .Remove(product);

            await this._dbContext
                .SaveChangesAsync();

            return this.RedirectToAction("Index", "Home");
        }
    }
}
