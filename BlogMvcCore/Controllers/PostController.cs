using Microsoft.AspNetCore.Mvc;
using BlogMvcCore.Services;

namespace BlogMvcCore.Controllers
{
    public class PostController : Controller
    {
        public readonly PostService postService;
        public readonly CommentService commentService;
        public PostController(PostService postService, CommentService commentService)
        {
            this.postService = postService;
            this.commentService = commentService;
        }

        public IActionResult ViewRecentAddedPostList() => View("ViewRecentAddedPostList", postService.ReturnPostList());

        [HttpPost]
        public IActionResult AddPost(string title, string postText, string ownerLogin)
        {
            postService.AddPost(title, postText, ownerLogin);
            return RedirectToAction("UserPage", "User");
        }

        public IActionResult DeletePost(long postID)
        {
            postService.DeletePost(postID);
            return RedirectToAction("UserPage", "User");
        }

        public IActionResult ViewPostAndComments(long postID) => View(postService.GetPostWithComments(postID, commentService));
    }
}
