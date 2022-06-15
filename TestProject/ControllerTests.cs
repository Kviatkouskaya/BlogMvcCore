using BlogMvcCore.Controllers;
using BlogMvcCore.DomainModel;
using BlogMvcCore.Helpers;
using BlogMvcCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace TestProject
{
    [TestClass]
    public class ControllerTests
    {
        private static IUserAction UserAction { get; set; }
        private Mock<Authentication> AuthMock = new(UserAction);
        private Mock<UserService> UserServiceMock = new(UserAction);
        private Mock<PostService> PostServiceMock = new(UserAction);
        private Mock<CommentService> CommentServiceMock = new(UserAction);

        private static ControllerContext CreateControllerContext(MockHttpSession mockHttpSession)
        {
            DefaultHttpContext httpContext = new();
            httpContext.Session = mockHttpSession;
            ControllerContext controllerContext = new();
            controllerContext.HttpContext = httpContext;
            return controllerContext;
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin1", "12345678")]
        public void VisitUserPage(string first, string second, string login, string password)
        {
            UserController userController = new(AuthMock.Object, UserServiceMock.Object,
                                                PostServiceMock.Object, CommentServiceMock.Object);
            MockHttpSession mockHttpSession = new();
            userController.ControllerContext = CreateControllerContext(mockHttpSession);
            User user = new(first, second, login, password);
            userController.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = userController.VisitUserPage(login);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin1", "12345678", "fakeLogin")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "")]
        [DataRow("Frank", "Right", "right1", "123qwe123", null)]
        public void VisitUserPageFail(string first, string second, string login,
                                      string password, string fakeLogin)
        {
            User user = new(first, second, login, password);
            UserServiceMock.Setup(x => x.VisitUserPage(fakeLogin)).Returns(user).Verifiable();
            UserController userController = new(AuthMock.Object, UserServiceMock.Object,
                                                PostServiceMock.Object, CommentServiceMock.Object);
            MockHttpSession mockHttpSession = new();
            userController.ControllerContext = CreateControllerContext(mockHttpSession);
            userController.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = userController.VisitUserPage(fakeLogin);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            UserServiceMock.VerifyAll();
        }

        [TestMethod]
        [DataRow("Post title", "Post text", "admin")]
        [DataRow("New Post title", "New post text", "adamsm")]
        [DataRow("Test title", "Test post text", "right")]
        public void AddPost(string title, string postText, string ownerLogin)
        {
            User user = new("testUser", "secondName", "user", "123123");
            AuthMock.Setup(x => x.CheckStringParams(title, postText, ownerLogin)).Returns(true).Verifiable();
            PostServiceMock.Setup(x => x.AddPost(title, postText, ownerLogin)).Verifiable();
            UserController userController = new(AuthMock.Object, UserServiceMock.Object,
                                                PostServiceMock.Object, CommentServiceMock.Object);

            var result = userController.AddPost(title, postText, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
            AuthMock.VerifyAll();
            PostServiceMock.VerifyAll();
        }
    }
}
