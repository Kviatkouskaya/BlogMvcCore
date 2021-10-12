using Microsoft.AspNetCore.Mvc;
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
        public IActionResult SignOut()
        {
            SessionHelper.SetUserAsJson(HttpContext.Session, "user", null);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CheckIn(string login, string password)
        {
            if (repContext.LoginUser(login, password) == 1)
            {
                var user = repContext.FindUser(login);
                SessionHelper.SetUserAsJson(HttpContext.Session, "user", user);
                return RedirectToAction("UserPage");
            }
            return RedirectToAction("SignIn");
        }

        public IActionResult ShowUsersList()
        {
            return View(repContext.ReturnUsersList());
        }

        public IActionResult VisitUserPage(string login)
        {
            User user = repContext.FindUser(login);
            user.Posts = repContext.ReturnUserPost(user);
            foreach (var item in user.Posts)
            {
                item.Comments = repContext.ReturnPostComment(item);
            }
            return View(user);
        }
        public IActionResult UserPage()
        {
            User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
            user.Posts = repContext.ReturnUserPost(user);
            foreach (var item in user.Posts)
            {
                item.Comments = repContext.ReturnPostComment(item);
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult AddPost(string title, string postText, string ownerLogin)
        {
            if (title != string.Empty && postText != string.Empty)
            {
                Post newPost = new()
                {
                    Author = repContext.FindUser(ownerLogin),
                    Title = title,
                    Text = postText,
                    Date = DateTime.Now.Date
                };
                repContext.AddPost(newPost);
            }
            return RedirectToAction("UserPage");
        }
        [HttpPost]
        public IActionResult AddComment(string commentText, long postID, string ownerLogin)
        {
            User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
            if (commentText != string.Empty)
            {
                Comment comment = new()
                {
                    Post = repContext.FindPost(postID),
                    Author = $"{user.FirstName} {user.SecondName}",
                    Text = commentText,
                    Date = DateTime.Now.Date
                };
                comment.Post.Author = repContext.FindUser(ownerLogin);
                repContext.AddComment(comment);
            }
            return RedirectToAction("UserPage");
        }
    }
}
