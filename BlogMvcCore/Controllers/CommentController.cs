using Microsoft.AspNetCore.Mvc;
using BlogMvcCore.Services;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;
using BlogMvcCore.DomainModel;

namespace BlogMvcCore.Controllers
{
    public class CommentController : Controller
    {
        private readonly CommentService commentService;
        public CommentController(CommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpPost]
        public IActionResult AddComment(string commentText, long postID, long parentID)
        {
            UserDomainModel user = SessionHelper.GetUserFromJson<UserDomainModel>(HttpContext.Session, "user");
            commentService.AddComment(commentText, postID, parentID, user);

            return RedirectToAction("ViewPostAndComments", "Post", new { postID });
        }

        public IActionResult EditComment(long commentID, string text, long postID) => View(new CommentDomainModel
        {
            ID = commentID,
            Text = text,
            Post = new PostDomainModel { ID = postID }
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
