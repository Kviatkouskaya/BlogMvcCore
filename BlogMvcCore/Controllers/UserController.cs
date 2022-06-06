using BlogMvcCore.DomainModel;
using BlogMvcCore.Services;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BlogMvcCore.Controllers
{
    public class UserController : Controller
    {
        private readonly Authentication authentication;
        public UserController(Authentication authentication)
        {
            this.authentication = authentication;
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
            var registrationCheck = authentication.CheckUserRegistration(first, second, login, password, repPassword);
            return registrationCheck ? RedirectToAction("SignIn") : RedirectToAction("Register");
        }

        public IActionResult SignIn()
        {
            return View();
        }

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
            return user == null ? RedirectToAction("SignIn") : RedirectToAction("UserPage");
        }

        /*
        public IActionResult ShowUsersList()
        {
            return View(repContext.ReturnUsersList());
        }

        public IActionResult ViewPostAndComments(long postID)
        {
            Post post = repContext.FindPost(postID);
            var commentList = repContext.ReturnPostComment(post);

            List<CommentWithLevel> commentWithLevels = new();
            FillCommentGen(commentWithLevels, commentList, 0, default);
            post.Comments = commentWithLevels;

            return View(post);
        }

        private void FillCommentGen(List<CommentWithLevel> finalList, List<Comment> commentList, int level, long parentID)
        {
            List<CommentWithLevel> commentWithLevels = new();
            List<Comment> childComment = commentList.Where(x => x.Parent == parentID).ToList();
            if (childComment.Count != 0)
            {
                foreach (var child in childComment)
                {
                    finalList.Add(new CommentWithLevel { Comment = child, Level = level });
                    var nextLevel = level + 1;
                    FillCommentGen(finalList, commentList, nextLevel, child.ID);
                    var nextCommentGeneration = commentList.Where(x => x.Parent == child.ID).ToList();
                }
            }
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

        */
        public IActionResult UserPage()
        {
            User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
            user.Posts = null;// repContext.ReturnUserPost(user);
            return View(user);
        }
        /*
        [HttpPost]
        public IActionResult AddPost(string title, string postText, string ownerLogin)
        {
            Authentication authentication = new();
            if (authentication.CheckStringParams(title, postText, ownerLogin))
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
            Authentication authentication = new();
            if (authentication.CheckStringParams(commentText))
            {
                Comment comment = new()
                {
                    Post = repContext.FindPost(postID),
                    Author = $"{user.FirstName} {user.SecondName}",
                    Parent = parentID,
                    Text = commentText,
                    Date = DateTime.Now.Date
                };
                comment.Post.Author = repContext.FindUser(user.Login);
                repContext.AddComment(comment);
            }
            return RedirectToAction("ViewPostAndComments", new { postID });
        }

        public IActionResult ViewRecentAddedPostList()
        {

            return View("ViewRecentAddedPostList", repContext.ReturnPostList());
        }

        */
    }
}