using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using BlogMvcCore.Models;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;

namespace BlogMvcCore.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserAction repContext;
        public UserController(IUserAction userAction)
        {
            repContext = userAction;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckRegister(string first, string second, string login,
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
                    return RedirectToAction("SignIn");
                }
            }
            return RedirectToAction("Register");
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckIn(string login, string password)
        {
            if (repContext.LoginUser(login, password) == 1)
            {
                var user = repContext.FindUser(login);
                HttpContext.Session.SetString("name", $"{user.FirstName} {user.SecondName}");
                SessionHelper.SetUserAsJson(HttpContext.Session, "user", user);
                return RedirectToAction("UserPage");
            }
            return RedirectToAction("SignIn");
        }

        public IActionResult ShowUsersList()
        {
            return View(repContext.ReturnUsersList());
        }

        public IActionResult UserPage()
        {
            User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
            List<Post> userPost = repContext.ReturnUserPost(user);
            return View(userPost);
        }
        [HttpPost]
        public IActionResult AddPost(string title, string postText)
        {
            if (title != string.Empty && postText != string.Empty)
            {
                Post newPost = new()
                {
                    Author = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user"),
                    Title = title,
                    Text = postText,
                    Date = DateTime.Now.Date
                };
                repContext.AddPost(newPost);
            }
            return RedirectToAction("UserPage");
        }
        [HttpPost]
        public IActionResult AddComment(string commentText, long postID)
        {
            if (commentText != string.Empty)
            {
                User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
                Comment comment = new()
                {
                    Post = repContext.FindPost(postID),
                    Author = HttpContext.Session.GetString("name"),
                    Text = commentText,
                    Date = DateTime.Now.Date
                };
                comment.Post.Author = user;
                repContext.AddComment(comment);
            }
            return RedirectToAction("UserPage");
        }
    }
}
