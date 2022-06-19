using BlogMvcCore.Controllers;
using BlogMvcCore.DomainModel;
using BlogMvcCore.Helpers;
using BlogMvcCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace TestProject.Controller_Tests
{
    [TestClass]
    public class PostTests
    {
        private static IPostAction PostAction { get; set; }
        private static IUserAction UserAction { get; set; }
        private static IComment CommentAction { get; set; }
        private Mock<PostService> PostServiceMock = new(PostAction, UserAction);
        private readonly Mock<CommentService> CommentService = new(CommentAction, PostAction, UserAction);

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
            User user = new("testUser", "secondName", "user", "123123");
            PostServiceMock.Setup(x => x.AddPost(title, postText, ownerLogin)).Verifiable();
            PostController controller = new(PostServiceMock.Object, CommentService.Object);

            var result = controller.AddPost(title, postText, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
            PostServiceMock.VerifyAll();
        }
    }
}
