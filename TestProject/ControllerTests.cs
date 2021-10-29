using BlogMvcCore.Controllers;
using BlogMvcCore.DomainModel;
using BlogMvcCore.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestProject
{
    [TestClass]
    public class ControllerTests
    {
        [TestMethod]
        [DataRow("Admin", "System", "admin", "12345678")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc")]
        [DataRow("Frank", "Right", "right1", "123qwe123")]
        public void CheckInSuccess(string first, string second, string login, string password)
        {
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.LoginUser(login, password)).Returns(true);
            mock.Setup(m => m.FindUser(login)).Returns(new User(first, second, login, password));
            MockHttpSession mockHttpSession = new();
            UserController controller = new(mock.Object);
            controller.ControllerContext = CreateControllerContext(mockHttpSession);

            var result = controller.CheckIn(login, password);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", newResult.ActionName);
            var sessionResult = mockHttpSession.GetString("user");
            Assert.IsNotNull(sessionResult);
            var userSession = JsonConvert.DeserializeObject<User>(sessionResult.ToString());
            Assert.AreEqual(first, userSession.FirstName);
            Assert.AreEqual(second, userSession.SecondName);
            Assert.AreEqual(login, userSession.Login);
            Assert.AreEqual(password, userSession.Password);
            mock.Verify();
        }

        [TestMethod]
        [DataRow("admin", "12345679")]
        [DataRow("", "qweasdzxc")]
        [DataRow("right", null)]
        public void CheckInUnsuccess(string login, string password)
        {
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.LoginUser(login, password)).Returns(false);
            UserController controller = new(mock.Object);

            var result = controller.CheckIn(login, password);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("SignIn", newResult.ActionName);
            mock.Verify();
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin1", "12345678", "12345678")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "qweasdzxc")]
        [DataRow("Frank", "Right", "right1", "123qwe123", "123qwe123")]
        public void RegisterTest(string first, string second, string login,
                                      string password, string repPassword)
        {
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.CheckLoginDuplicate(login)).Returns(0);
            UserController controller = new(mock.Object);

            var result = controller.CheckRegister(first, second, login, password, repPassword);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual(newResult.ActionName, "SignIn");
            mock.Verify();
        }

        [TestMethod]
        [DataRow("Admin", "System", null, "12345678", "12345678")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "qweasdzx11")]
        [DataRow("Frank", "Right", "right1", "", "123qwe123")]
        public void RegisterUnsuccess(string first, string second, string login,
                                      string password, string repPassword)
        {
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.CheckLoginDuplicate(login)).Returns(1);
            UserController controller = new(mock.Object);

            var result = controller.CheckRegister(first, second, login, password, repPassword);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("Register", newResult.ActionName);
            mock.Verify();
        }

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
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc")]
        [DataRow("Frank", "Right", "right1", "123qwe123")]
        public void VisitUserPage(string first, string second, string login, string password)
        {
            Mock<IUserAction> mock = new();
            UserController controller = new(mock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            User user = new(first, second, login, password);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.VisitUserPage(login);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
            mock.Verify();
        }

        [TestMethod]
        [DataRow("Comment text", "admin")]
        [DataRow("Test text", "right")]
        [DataRow("Comment", "frank")]
        public void AddComment(string commentText, string ownerLogin)
        {
            User user = new("admin", "secondName", ownerLogin, "123123");
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.FindUser(user.Login)).Returns(user);
            mock.Setup(m => m.FindPost(0)).Returns(new Post());
            mock.Setup(m => m.AddComment(new Comment()));
            UserController controller = new(mock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.AddComment(commentText, 0, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
        }

        [TestMethod]
        [DataRow("Comment text", "admin", "userLogin")]
        [DataRow("Test text", "right", "frank")]
        [DataRow("Comment", "frank", "login")]
        public void AddCommentTest(string commentText, string ownerLogin, string userLogin)
        {
            User user = new("admin", "secondName", userLogin, "123123");
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.FindUser(user.Login)).Returns(user);
            mock.Setup(m => m.FindPost(0)).Returns(new Post());
            mock.Setup(m => m.AddComment(new Comment()));
            UserController controller = new(mock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.AddComment(commentText, 0, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("VisitUserPage", redirect.ActionName);
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin1", "12345678", "fakeLogin")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "")]
        [DataRow("Frank", "Right", "right1", "123qwe123", null)]
        public void VisitUserPageUnsuccess(string first, string second, string login,
                                           string password, string fakeLogin)
        {
            User user = new(first, second, login, password);
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.FindUser(login)).Returns(user);
            mock.Setup(m => m.ReturnUserPost(user)).Returns(new List<Post>());
            UserController controller = new(mock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.VisitUserPage(fakeLogin);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            mock.Verify();
        }

        [TestMethod]
        [DataRow("Post title", "Post text", "admin")]
        [DataRow("New Post title", "New post text", "adamsm")]
        [DataRow("Test title", "Test post text", "right")]
        public void AddPost(string title, string postText, string ownerLogin)
        {
            User user = new("testUser", "secondName", "user", "123123");
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.FindUser(ownerLogin)).Returns(user);
            mock.Setup(m => m.AddPost(new Post()));
            UserController controller = new(mock.Object);

            var result = controller.AddPost(title, postText, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
            mock.Verify();
        }
    }
}
