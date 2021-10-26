using Microsoft.AspNetCore.Mvc;
using System;
using BlogMvcCore.DomainModel;
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
            bool stringCheck = CheckStringParams(first, second, login, password, repPassword);
            if (password == repPassword && stringCheck &&
                repContext.CheckLoginDuplicate(login) == 0)
            {
                repContext.Register(new User(first, second, login, password));
                return RedirectToAction("SignIn");
            }
            return RedirectToAction("Register");
        }

        private static bool CheckStringParams(params string[] input)
        {
            foreach (var item in input)
            {
                if (item == string.Empty || string.IsNullOrWhiteSpace(item))
                {
                    return false;
                }
            }
            return true;
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
            if (repContext.LoginUser(login, password) == 1 && CheckStringParams(login, password))
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
            if (login == SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user").Login)
            {
                return RedirectToAction("UserPage");
            }
            User user = FillPostsComments(repContext.FindUser(login));
            return View(user);
        }

        private User FillPostsComments(User user)
        {
            user.Posts = repContext.ReturnUserPost(user);
            foreach (var item in user.Posts)
            {
                item.Comments = repContext.ReturnPostComment(item);
            }
            return user;
        }

        public IActionResult UserPage()
        {
            User user = FillPostsComments(SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user"));
            return View(user);
        }

        [HttpPost]
        public IActionResult AddPost(string title, string postText, string ownerLogin)
        {
            if (CheckStringParams(title, postText, ownerLogin))
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
            if (CheckStringParams(commentText))
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
            return user.Login == ownerLogin ? RedirectToAction("UserPage") :
                                              RedirectToAction("VisitUserPage", new { login = ownerLogin });
        }
    }
}