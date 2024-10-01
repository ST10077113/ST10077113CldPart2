using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using POECLDPart1.Models;
using POECLDPart1.Services;
using System.Security.Claims;

namespace POECLDPart1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        // get  account/register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // post  account/register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = await _authService.RegisterAsync(model.Email, model.Password, model.FullName);
                if (success)
                    return RedirectToAction("Login"); // Correct redirect after successful registration
                ModelState.AddModelError("", "User already exists");
            }
            return View(model);
        }

        // get account/login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // post  account/login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) // Added [HttpPost] attribute
        {
            if (ModelState.IsValid)
            {
                var user = await _authService.LoginAsync(model.Email, model.Password);
                if (user != null)
                {
                    // create claims
                    var claim = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe // This should already be correct
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), authProperties);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(model);
        }

        // post account/logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account"); // Redirect to Account/Login
        }

    }
}
