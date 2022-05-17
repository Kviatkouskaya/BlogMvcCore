using BlogMvcCore.DomainModel;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

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
            if (password == repPassword && stringCheck)
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

        public new IActionResult SignOut()
        {
            SessionHelper.SetUserAsJson(HttpContext.Session, "user", null);
            repContext.Dispose();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CheckIn(string login, string password)
        {
            if (CheckStringParams(login, password) && repContext.LoginUser(login, password))
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

        public IActionResult ViewPostAndComments(long postID)
        {
            Post post = repContext.FindPost(postID);
            post.Comments = repContext.ReturnPostComment(post);
            return View(post);
        }

        public IActionResult VisitUserPage(string login)
        {
            if (login == SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user").Login)
            {
                return RedirectToAction("UserPage");
            }
            User user = repContext.FindUser(login);
            user.Posts = repContext.ReturnUserPost(user);
            return View(user);
        }

        public User FillPostsComments(User user)
        {
            if (user is not null)
            {
                user.Posts = repContext.ReturnUserPost(user);
                foreach (var item in user.Posts)
                {
                    item.Comments = repContext.ReturnPostComment(item);
                }
            }
            return user;
        }


        public IActionResult UserPage()
        {
            User user = FillPostsComments(SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user"));
            user.Posts = repContext.ReturnUserPost(user);
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
        public IActionResult AddComment(string commentText, long postID, long parentID)
        {
            User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
            if (CheckStringParams(commentText))
            {
                Comment comment = new()
                {
                    Post = repContext.FindPost(postID),
                    Author = $"{user.FirstName} {user.SecondName}",
                    Parent = parentID == postID ? 0 : parentID,
                    Text = commentText,
                    Date = DateTime.Now.Date
                };
                comment.Post.Author = repContext.FindUser(user.Login);
                repContext.AddComment(comment);
            }
            return RedirectToAction("ViewPostAndComments", new { postID });
        }
    }
}