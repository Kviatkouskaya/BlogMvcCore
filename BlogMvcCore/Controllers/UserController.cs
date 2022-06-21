using BlogMvcCore.DomainModel;
using BlogMvcCore.Services;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogMvcCore.Controllers
{
    public class UserController : Controller
    {
        public readonly UserService userService;
        public readonly CommentService commentService;
        public UserController(UserService userService,
                              CommentService commentService)
        {
            this.userService = userService;
            this.commentService = commentService;
        }

        public IActionResult VisitUserPage(string login)
        {
            return login == SessionHelper.GetUserFromJson<UserDomain>(HttpContext.Session, "user").Login ?
                            RedirectToAction("UserPage") : View(userService.VisitUserPage(login));
        }
        public IActionResult UserPage()
        {
            var userLogin = SessionHelper.GetUserFromJson<UserDomain>(HttpContext.Session, "user").Login;
            return View(userService.VisitUserPage(userLogin));
        }

        public IActionResult ShowUsersList() => View(userService.ReturnUsers());
    }
}