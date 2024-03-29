﻿using Microsoft.AspNetCore.Mvc;
using BlogMvcCore.Services;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;

namespace BlogMvcCore.Controllers
{
    public class AuthenticationController : Controller
    {
        public readonly AuthenticationService authenticationService;
        public AuthenticationController(AuthenticationService authentication) => this.authenticationService = authentication;
        public IActionResult Index() => View();

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult CheckRegister(string firstName, string secondName, string login,
                                           string password)
        {
            var registrationCheck = authenticationService.AddUser(firstName, secondName, login, password);
            return registrationCheck ? RedirectToAction("SignIn") : RedirectToAction("Register");
        }

        public IActionResult SignIn() => View();

        public new IActionResult SignOut()
        {
            SessionHelper.SetUserAsJson(HttpContext.Session, "user", null);
            authenticationService.SignOut();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CheckIn(string login, string password)
        {
            var user = authenticationService.CheckIn(login, password);
            SessionHelper.SetUserAsJson(HttpContext.Session, "user", user);

            return user == null ? RedirectToAction("SignIn") : RedirectToAction("UserPage", "User");
        }
    }
}
