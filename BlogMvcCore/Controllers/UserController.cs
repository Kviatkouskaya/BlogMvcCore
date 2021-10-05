using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using BlogMvcCore.Models;
using BlogMvcCore.Helpers;

namespace BlogMvcCore.Controllers
{
    public class UserController : Controller
    {
        private readonly Repository repContext;
        public UserController(Repository repository)
        {
            repContext = repository;
        }
        public ActionResult Index()
        {
            int dayTime = DateTime.Now.Hour;
            ViewBag.Greeting = dayTime < 12 && dayTime > 6 ? $"Good morning!" :
                              (dayTime < 18 ? "Good afternoon!" : "Good evening!");
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
            var count = repContext.CheckLoginDuplicate(login);
            if (password == repPassword && count == 0)
            {
                if (first != string.Empty && second != string.Empty &&
                    login != string.Empty && password != string.Empty && repPassword != string.Empty)
                {
                    User user = new(first, second, login, password);
                    repContext.Register(user);
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
            if (repContext.LoginUser(login, password) == 1)
            {
                SessionHelper.SetUserAsJson(HttpContext.Session, "user", repContext.FindUser(login));
                return Redirect("/User/UserPage");
            }
            return Redirect("/User/SignIn");
        }
        public ViewResult UserPage()
        {
            User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
            ViewBag.UserName = $"{user.FirstName} {user.SecondName}";
            List<Post> userPost = repContext.ReturnUserPost(user);
            if (userPost == null)
            {
                return View();
            }
            return View(userPost);
        }
        [HttpPost]
        public ActionResult AddPost(string title, string postText)
        {
            if (title != string.Empty && postText != string.Empty)
            {
                User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
                Post newPost = new()
                {
                    Author = user,
                    Title = title,
                    Text = postText,
                    Date = DateTime.Now.Date
                };
                repContext.AddPost(newPost);
            }
            return Redirect("/User/UserPage");
        }
        [HttpPost]
        public ActionResult AddComment(string commentText, long postID)
        {
            if (commentText != string.Empty)
            {
                User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
                Comment comment = new()
                {
                    Post = repContext.FindPost(postID),
                    Author = $"{user.FirstName} {user.SecondName}",
                    Text = commentText,
                    Date = DateTime.Now.Date
                };
                comment.Post.Author = user;
                repContext.AddComment(comment);
            }
            return Redirect("/User/UserPage");
        }
    }
}
