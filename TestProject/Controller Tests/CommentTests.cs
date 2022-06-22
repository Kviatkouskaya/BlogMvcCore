using BlogMvcCore.Controllers;
using BlogMvcCore.DomainModel;
using BlogMvcCore.Storage;
using BlogMvcCore.Helpers;
using BlogMvcCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestProject.Controller_Tests
{
    [TestClass]
    public class CommentTests
    {
        private static ICommentRepository CommentRepository { get; set; }
        private static IPostRepository PostRepository { get; set; }
        private static IUserRepository UserRepository { get; set; }
        private readonly Mock<CommentService> CommentService = new(CommentRepository, PostRepository, UserRepository);

        private static ControllerContext CreateControllerContext(MockHttpSession mockHttpSession)
        {
            DefaultHttpContext httpContext = new();
            httpContext.Session = mockHttpSession;
            ControllerContext controllerContext = new();
            controllerContext.HttpContext = httpContext;
            return controllerContext;
        }

        [TestMethod]
        [DataRow("Comment text", "admin", 1, 21)]
        [DataRow("Test text", "right", 3, 2)]
        [DataRow("Comment", "frank", 4, 5)]
        public void AddComment(string commentText, string ownerLogin, long postID, long parentID)
        {
            UserDomain user = new("admin", "secondName", ownerLogin, "123123");
            CommentController controller = new(CommentService.Object);
            CommentService.Setup(x => x.AddComment(commentText, postID, parentID, user));
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.AddComment(commentText, postID, parentID);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("ViewPostAndComments", redirect.ActionName);
        }
    }
}
