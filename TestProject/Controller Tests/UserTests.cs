using BlogMvcCore.Controllers;
using BlogMvcCore.DomainModel;
using BlogMvcCore.Helpers;
using BlogMvcCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestProject.Controller_Tests
{
    [TestClass]
    public class UserTests
    {
        private static IUserAction UserAction { get; set; }
        private static IPostAction PostAction { get; set; }
        private static IComment CommentAction { get; set; }
        private readonly Mock<UserService> UserServiceMock = new(UserAction, PostAction);
        private readonly Mock<CommentService> CommentServiceMock = new(CommentAction, PostAction, UserAction);

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
        public void UserPage(string first, string second, string login, string password)
        {
            User user = new(first, second, login, password);
            UserController controller = new(UserServiceMock.Object, CommentServiceMock.Object);
            MockHttpSession mockHttpSession = new();
            mockHttpSession.SetUserAsJson("user", user);
            controller.ControllerContext = CreateControllerContext(mockHttpSession);

            var result = controller.VisitUserPage(login);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin1", "12345678")]
        public void VisitUserPage(string first, string second, string login, string password)
        {
            UserController controller = new(UserServiceMock.Object, CommentServiceMock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            User user = new(first, second, login, password);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.VisitUserPage(login);

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
            UserController controller = new(UserServiceMock.Object, CommentServiceMock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.VisitUserPage(fakeLogin);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            UserServiceMock.VerifyAll();
        }
    }
}
