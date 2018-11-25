using System.Threading.Tasks;
using AutoMapper;
using Eventures.Data;
using Eventures.Models;
using Eventures.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventures.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EventuresDbContext _dbContext;
        private readonly IMapper _mapper;

        public UsersController(
            SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, EventuresDbContext dbContext, IMapper mapper)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError(string.Empty, "You are already signed in.");
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await this._signInManager
                    .PasswordSignInAsync(viewModel.Username, viewModel.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Wrong username or password!");
            }

            return View(viewModel);
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError(string.Empty, "You are already signed in.");
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var usersExists = await this._dbContext
                    .Users
                    .FirstOrDefaultAsync(x => x.UserName == viewModel.Username) != null;
                if (usersExists)
                {
                    ModelState.AddModelError(string.Empty, $"User \"{viewModel.Username}\" already exists!");
                    return View(viewModel);
                }
                var emailExists = await this._dbContext
                    .Users
                    .FirstOrDefaultAsync(x => x.Email == viewModel.Email) != null;
                if (emailExists)
                {
                    ModelState.AddModelError(string.Empty, $"Email \"{viewModel.Email}\" is already taken!");
                    return View(viewModel);
                }

                var user = this._mapper.Map<RegisterViewModel, User>(viewModel);

                var roleName = "User";
                if (!await this._dbContext.Users.AnyAsync())
                {
                    roleName = "Admin";
                }

                var result = await this._userManager.CreateAsync(user, viewModel.Password);
                if (result.Succeeded)
                {
                    //// app.UseSeedRoles() middleware creates Roles User and Admin
                    //if (!await this._roleManager.RoleExistsAsync(roleName))
                    //{
                    //    await this._roleManager.CreateAsync(new IdentityRole(roleName));
                    //}

                    await this._userManager.AddToRoleAsync(user, roleName);
                    await this._signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}