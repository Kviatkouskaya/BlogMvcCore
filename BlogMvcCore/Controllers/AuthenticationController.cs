using Microsoft.AspNetCore.Mvc;
using BlogMvcCore.Services;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;

namespace BlogMvcCore.Controllers
{
    public class AuthenticationController : Controller
    {
        public readonly Authentication authentication;
        public AuthenticationController(Authentication authentication) => this.authentication = authentication;
        public IActionResult Index() => View();

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult CheckRegister(string first, string second, string login,
                                           string password, string repPassword)
        {
            var registrationCheck = authentication.CheckUserRegistration(first, second, login, password, repPassword);
            return registrationCheck ? RedirectToAction("SignIn") : RedirectToAction("Register");
        }

        public IActionResult SignIn() => View();

        public new IActionResult SignOut()
        {
            SessionHelper.SetUserAsJson(HttpContext.Session, "user", null);
            authentication.SignOut();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CheckIn(string login, string password)
        {
            var user = authentication.CheckIn(login, password);
            SessionHelper.SetUserAsJson(HttpContext.Session, "user", user);

            return user == null ? RedirectToAction("SignIn") : RedirectToAction("UserPage", "User");
        }
    }
}
