using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Barebone.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Barebone.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _siteUserName;
        private readonly string _siteUserPassword;
        public AccountController(IConfiguration configuration)
        {
            _siteUserName = configuration["SiteUser:UserName"];
            _siteUserPassword = configuration["SiteUser:Password"];
        }

        [HttpGet]
        [ActionName("LogIn")]
        [AllowAnonymous]
        public async Task<IActionResult> LogInAsync()
        {
            return await Task.Run(View);
        }

        [HttpPost]
        [ActionName("LogIn")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogInAsync(LogInViewModel logIn)
        {
            // Check required fields, if any empty return to login page
            if (!ModelState.IsValid)
            {
                logIn.Message = "Required data missing";
                ModelState.AddModelError("BadUserPassword", logIn.Message);
                return await Task.Run(() => View(logIn));
            }

            if (logIn.UserName == _siteUserName)
            {
                var passwordHasher = new PasswordHasher<string>();
                if (passwordHasher.VerifyHashedPassword(null, _siteUserPassword, logIn.Password) == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, logIn.UserName)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return await Task.Run(() => RedirectToAction("Index", "Barebone"));
                }
            }
            logIn.Message = "Invalid attempt";
            return await Task.Run(() => View(logIn));
        }

        [HttpGet]
        [ActionName("LogOut")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LogIn","Account");
        }

        #region PasswordHasher

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordHasherAsync()
        {
            return await Task.Run(View);
        }
        
        [HttpPost]
        [ActionName("PasswordHasher")]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordHasherAsync(PasswordHasherViewModel passwordHasher)
        {
            ViewBag.PasswordResult = new PasswordHasher<string>().HashPassword(null, passwordHasher.PasswordToHash);
            return await Task.Run(View);
        }

        #endregion
    }

}