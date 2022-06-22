using BlogMvcCore.Controllers;
using BlogMvcCore.DomainModel;
using BlogMvcCore.Storage;
using BlogMvcCore.Services;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestProject.Controller_Tests
{
    [TestClass]
    public class PostTests
    {
        private static IPostRepository PostRepository { get; set; }
        private static ICommentRepository CommentRepository { get; set; }
        private Mock<PostService> PostServiceMock = new(PostRepository);
        private readonly Mock<CommentService> CommentService = new(CommentRepository, PostRepository);

        private static ControllerContext CreateControllerContext(MockHttpSession mockHttpSession)
        {
            DefaultHttpContext httpContext = new();
            httpContext.Session = mockHttpSession;
            ControllerContext controllerContext = new();
            controllerContext.HttpContext = httpContext;
            return controllerContext;
        }

        [TestMethod]
        [DataRow("Post title", "Post text", "admin")]
        [DataRow("New Post title", "New post text", "adamsm")]
        [DataRow("Test title", "Test post text", "right")]
        public void AddPost(string title, string postText, string ownerLogin)
        {
            UserDomain user = new("testUser", "secondName", "user", "123123");
            PostServiceMock.Setup(x => x.AddPost(title, postText, user));
            PostController controller = new(PostServiceMock.Object, CommentService.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.AddPost(title, postText);//, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
        }
    }
}
