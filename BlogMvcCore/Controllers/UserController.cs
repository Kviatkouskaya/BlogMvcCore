using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogMvcCore.Models;

namespace BlogMvcCore.Controllers
{
    public class UserController : Controller
    {
        private readonly Repository RepContext;
        public UserController(Repository repository)
        {
            RepContext = repository;
        }
        public ActionResult Index()
        {
            int dayTime = DateTime.Now.Hour;
            ViewBag.Greeting = dayTime < 12 && dayTime > 6 ? $"Good morning!" :
                              (dayTime < 18 ? "Good afternoon!" : "Good evening!");
            return View();
        }

        public ActionResult Hello(string name)
        {
            ViewBag.Greeting = name == string.Empty ? "Hello!" : $"Hello, {name}!";
            return View();
        }

        public ViewResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckRegister(string first, string second, string login,
                                          string password, string repPassword)
        {
            if (password == repPassword)
            {
                User user = new(first, second, login, password);
                if (RepContext.Register(user))
                {
                    return Redirect("/User/SignIn");
                }
            }
            return Redirect("/User/Register");
        }

        public ViewResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckIn(string login, string password)
        {
            bool state = RepContext.LoginUser(login, password);
            if (state)
            {
                //Session["Login"] = Request.Form["Login"];
                return Redirect("/User/UserPage");
            }
            return Redirect("/User/SignIn");
        }
        public ViewResult UserPage()
        {
            //ViewBag.UserName = RepContext.FindUser(Session["Login"].ToString());
            return View();
        }
        [HttpPost]
        public ActionResult AddPost(string topic, string postText)
        {
            Post newPost = new()
            {
                Date = DateTime.Now.Date,
                Author = new User("first", "second", "login", "pass"), //UserDB.ReturnUser(Session["Login"].ToString()),
                Title = topic,
                Text = postText
            };
            RepContext.AddPost(newPost);
            return Redirect("/User/UserPage");
        }
    }
}
