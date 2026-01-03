using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using TimesheetPro.Core.Domain.IdentityEntities;
using TimesheetPro.Core.Enums;
using TimesheetPro.UI.Models;

namespace TimesheetPro.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var loginView = new LoginViewModel { ReturnUrl = returnUrl };
            return View(loginView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginView)
        {
            if (!ModelState.IsValid)
            {
                return View(loginView);
            }

            var user = await _userManager.FindByEmailAsync(loginView.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View(loginView);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginView.Password,
                isPersistent: false,
                lockoutOnFailure:false
                );

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(loginView.ReturnUrl) && Url.IsLocalUrl(loginView.ReturnUrl))
                {
                    return Redirect(loginView.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Project");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View(loginView);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }



        [Authorize(Roles = nameof(AppRoles.Admin))]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                Role = AppRoles.Consultant
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(AppRoles.Admin))]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            //create user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            //role
            var roleName = model.Role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = roleName});
            }

            await _userManager.AddToRoleAsync(user, roleName);



            //Do not automatically log in new users; Admin should remain logged in
            TempData["Success"] = $"User {model.Email} created with role {roleName}";

            //Return to the same page for easy creation of the next user
            return RedirectToAction(nameof(Register));
        }
    }
}
