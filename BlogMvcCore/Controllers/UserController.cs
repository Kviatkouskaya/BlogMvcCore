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
            return login == SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user").Login ?
                            RedirectToAction("UserPage") : View(userService.VisitUserPage(login));
        }

        public IActionResult UserPage()
        {
            var userLogin = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user").Login;
            return View(userService.VisitUserPage(userLogin));
        }

        public IActionResult ShowUsersList() => View(userService.ReturnUsers());



        [HttpPost]
        public IActionResult AddComment(string commentText, long postID, long parentID)
        {
            User user = SessionHelper.GetUserFromJson<User>(HttpContext.Session, "user");
            commentService.AddComment(commentText, postID, parentID, user);

            return RedirectToAction("ViewPostAndComments", "Post", new { postID });
        }

        public IActionResult EditComment(long commentID, string text, long postID) => View(new Comment
        {
            ID = commentID,
            Text = text,
            Post = new Post { ID = postID }
        });

        public IActionResult UpdateComment(long commentID, string text, long postID)
        {
            commentService.UpdateComment(commentID, text);
            return RedirectToAction("ViewPostAndComments", "Post", new { postID });
        }

        public IActionResult DeleteComment(long postID, long commentID)
        {
            commentService.DeleteComment(commentID);
            return RedirectToAction("ViewPostAndComments", "Post", new { postID });
        }
    }
}