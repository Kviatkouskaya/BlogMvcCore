using BlogMvcCore.DomainModel;
using BlogMvcCore.Services;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogMvcCore.Controllers
{
    public class UserController : Controller
    {
        public readonly Authentication authentication;
        public readonly UserService userService;
        public readonly PostService postService;
        public readonly CommentService commentService;
        public UserController(Authentication authentication,
                              UserService userService,
                              PostService postService,
                              CommentService commentService)
        {
            this.authentication = authentication;
            this.userService = userService;
            this.postService = postService;
            this.commentService = commentService;
        }

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

            return user == null ? RedirectToAction("SignIn") : RedirectToAction("UserPage");
        }

        public IActionResult VisitUserPage(string login)
        {
            return login == SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user").Login ?
                            RedirectToAction("UserPage") : View(userService.VisitUserPage(login));
        }

        public IActionResult UserPage()
        {
            var userLogin = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user").Login;

            return View(userService.VisitUserPage(userLogin));
        }

        public IActionResult ShowUsersList() => View(userService.ReturnUsers());

        public IActionResult ViewRecentAddedPostList() => View("ViewRecentAddedPostList", postService.ReturnPostList());

        [HttpPost]
        public IActionResult AddPost(string title, string postText, string ownerLogin)
        {
            if (authentication.CheckStringParams(title, postText, ownerLogin))
                postService.AddPost(title, postText, ownerLogin);

            return RedirectToAction("UserPage");
        }

        public IActionResult DeletePost(long postID)
        {
            postService.DeletePost(postID);
            return RedirectToAction("UserPage");
        }

        public IActionResult ViewPostAndComments(long postID) => View(postService.GetPostWithComments(postID, commentService));

        [HttpPost]
        public IActionResult AddComment(string commentText, long postID, long parentID)
        {
            User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
            if (authentication.CheckStringParams(commentText))
                commentService.AddComment(commentText, postID, parentID, user);

            return RedirectToAction("ViewPostAndComments", new { postID });
        }
    }
}