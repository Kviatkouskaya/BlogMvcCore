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
        public void CheckIn(string first, string second, string login, string password)
        {
            Mock<IUserAction> mock = new();
            User user = new(first, second, login, password);
            mock.Setup(m => m.LoginUser(login, password)).Returns(true).Verifiable();
            mock.Setup(m => m.FindUser(login)).Returns(user).Verifiable();
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
            Assert.AreEqual(user, userSession);
            mock.VerifyAll();
        }

        [TestMethod]
        [DataRow("admin", "12345679")]
        [DataRow("adams1", "qweasdzxc")]
        [DataRow("ight1", "123qwe123")]
        public void CheckInFail(string login, string password)
        {
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.LoginUser(login, password)).Returns(false).Verifiable();
            UserController controller = new(mock.Object);

            var result = controller.CheckIn(login, password);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("SignIn", newResult.ActionName);
            mock.VerifyAll();
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin1", "12345678", "12345678")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "qweasdzxc")]
        [DataRow("Frank", "Right", "right1", "123qwe123", "123qwe123")]
        public void Register(string first, string second, string login,
                                      string password, string repPassword)
        {
            Mock<IUserAction> mock = new();
            UserController controller = new(mock.Object);

            var result = controller.CheckRegister(first, second, login, password, repPassword);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual(newResult.ActionName, "SignIn");
            mock.VerifyAll();
        }

        [TestMethod]
        [DataRow("Admin", "System", "", "12345678", "12345678")]
        [DataRow("Adam", "", "adamsm1", "qweasdzxc", "qweasdzxc")]
        [DataRow("Frank", "Right", "right1", "123qwe123", "")]
        public void RegisterFail(string first, string second, string login,
                                 string password, string repPassword)
        {
            Mock<IUserAction> mock = new();
            UserController controller = new(mock.Object);

            var result = controller.CheckRegister(first, second, login, password, repPassword);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("Register", newResult.ActionName);
            mock.VerifyAll();
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
        }

        [TestMethod]
        [DataRow("Comment text", "admin")]
        [DataRow("Test text", "right")]
        [DataRow("Comment", "frank")]
        public void AddComment(string commentText, string ownerLogin)
        {
            User user = new("admin", "secondName", ownerLogin, "123123");
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.FindUser(user.Login)).Returns(user).Verifiable();
            mock.Setup(m => m.FindPost(0)).Returns(new Post()).Verifiable();
            mock.Setup(m => m.AddComment(It.Is<Comment>(c => c.Post != null &&
                                                             c.Post.Author.Login == ownerLogin))).
                                                             Verifiable();
            UserController controller = new(mock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.AddComment(commentText, 0);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("ViewPostAndComments", redirect.ActionName);
            mock.VerifyAll();
        }

        [TestMethod]
        [DataRow("Interesting topic!", "admin", "userLogin")]
        [DataRow("Hello...", "right", "frank")]
        [DataRow("Comment for comment", "frank", "login")]
        public void AddCommentTest(string commentText, string ownerLogin, string userLogin)
        {
            User user = new("admin", "secondName", userLogin, "123123");
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.FindUser(userLogin)).Returns(user).Verifiable();
            mock.Setup(m => m.FindPost(0)).Returns(new Post()).Verifiable();
            mock.Setup(m => m.AddComment(It.Is<Comment>(c => c.Post != null &&
                                                             c.Author != null))).
                                                             Verifiable();
            UserController controller = new(mock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.AddComment(commentText, 0);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("ViewPostAndComments", redirect.ActionName);
            mock.VerifyAll();
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin1", "12345678", "fakeLogin")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "")]
        [DataRow("Frank", "Right", "right1", "123qwe123", null)]
        public void VisitUserPageFail(string first, string second, string login,
                                      string password, string fakeLogin)
        {
            User user = new(first, second, login, password);
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.FindUser(fakeLogin)).Returns(user).Verifiable();
            mock.Setup(m => m.ReturnUserPost(user)).Returns(new List<Post>()).Verifiable();
            UserController controller = new(mock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.VisitUserPage(fakeLogin);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            mock.VerifyAll();
        }

        [TestMethod]
        [DataRow("Post title", "Post text", "admin")]
        [DataRow("New Post title", "New post text", "adamsm")]
        [DataRow("Test title", "Test post text", "right")]
        public void AddPost(string title, string postText, string ownerLogin)
        {
            User user = new("testUser", "secondName", "user", "123123");
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.FindUser(ownerLogin)).Returns(user).Verifiable();
            mock.Setup(m => m.AddPost(It.Is<Post>(p => p.Text == postText))).Verifiable();
            UserController controller = new(mock.Object);

            var result = controller.AddPost(title, postText, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
            mock.VerifyAll();
        }
    }
}
