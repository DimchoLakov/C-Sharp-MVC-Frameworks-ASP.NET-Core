using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Eventures.Data;
using Eventures.Models;
using Eventures.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
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
                    .PasswordSignInAsync(viewModel.Username, viewModel.Password, isPersistent: viewModel.RememberMe, lockoutOnFailure: false);

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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var username = info.Principal.FindFirstValue(ClaimTypes.Surname);
                var user = new User { UserName = username, Email = email };

                var roleName = "User";
                if (!await this._dbContext.Users.AnyAsync())
                {
                    roleName = "Admin";
                }

                var registrationResult = await _userManager.CreateAsync(user);
                if (registrationResult.Succeeded)
                {
                    var loginResult = await _userManager.AddLoginAsync(user, info);
                    if (loginResult.Succeeded)
                    {
                        await this._signInManager.SignInAsync(user, isPersistent: false);
                        await this._userManager.AddToRoleAsync(user, roleName);
                        return RedirectToAction("Index", "Home");
                    }
                }

                foreach (var error in registrationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult FbLogin(string provider, string returnUrl = null)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", Url.Action("ExternalLoginCallback", "Users"));
            return Challenge(properties, provider);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllUsers()
        {
            var allUsers = await this._dbContext
                .Users
                .Select(x => new UserViewModel()
                {
                    Id = x.Id,
                    Username = x.UserName
                })
                .ToListAsync();

            return View(allUsers);
        }


        [HttpPost]
        public async Task<IActionResult> PromoteUser(UserIdViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this._userManager.Users.FirstOrDefaultAsync(x => x.Id == model.Id);

                var role = await this._userManager.GetRolesAsync(user);

                var userRole = await this._dbContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == model.Id);
                this._dbContext.UserRoles.Remove(userRole);

                await this._userManager.AddToRoleAsync(user, "Admin");

                return RedirectToAction("AllUsers", "Users");
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> DemoteUser(UserIdViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this._userManager.Users.FirstOrDefaultAsync(x => x.Id == model.Id);

                var role = await this._userManager.GetRolesAsync(user);

                var userRole = await this._dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId == model.Id);
                this._dbContext.UserRoles.Remove(userRole);

                await this._userManager.AddToRoleAsync(user, "User");

                return RedirectToAction("AllUsers", "Users");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}