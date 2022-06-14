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
        public readonly PostService postService;
        public readonly CommentService commentService;
        public UserController(UserService userService,
                              PostService postService,
                              CommentService commentService)
        {
            this.userService = userService;
            this.postService = postService;
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

        public IActionResult ViewRecentAddedPostList() => View("ViewRecentAddedPostList", postService.ReturnPostList());

        [HttpPost]
        public IActionResult AddPost(string title, string postText, string ownerLogin)
        {
            postService.AddPost(title, postText, ownerLogin);
            return RedirectToAction("UserPage");
        }
        [HttpDelete]
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
            commentService.AddComment(commentText, postID, parentID, user);

            return RedirectToAction("ViewPostAndComments", new { postID });
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
            return RedirectToAction("ViewPostAndComments", new { postID });
        }

        public IActionResult DeleteComment(long postID, long commentID)
        {
            commentService.DeleteComment(commentID);
            return RedirectToAction("ViewPostAndComments", new { postID });
        }
    }
}