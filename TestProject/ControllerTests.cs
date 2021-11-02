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
            mock.Setup(m => m.LoginUser(It.Is<string>(l => l.Length >= 5 &&
                                                         !string.IsNullOrEmpty(l) &&
                                                         l == login),
                                        It.Is<string>(p => p.Length >= 8 &&
                                                         !string.IsNullOrEmpty(p) &&
                                                         p == password))).Returns(false).Verifiable();
            mock.Setup(m => m.FindUser(It.Is<string>(s => s.Length >= 5))).Returns(new User(first, second, login, password)).Verifiable();
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
            mock.VerifyAll();
        }

        [TestMethod]
        [DataRow("admin", "12345679")]
        [DataRow("adams1", "qweasdzxc")]
        [DataRow("ight1", "123qwe123")]
        public void CheckInUnsuccess(string login, string password)
        {
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.LoginUser(It.Is<string>(l => l.Length >= 5 &&
                                                         !string.IsNullOrEmpty(l) &&
                                                         l == login),
                                        It.Is<string>(p => p.Length >= 8 &&
                                                         !string.IsNullOrEmpty(p) &&
                                                         p == password))).Returns(false).Verifiable();
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
        public void RegisterTest(string first, string second, string login,
                                      string password, string repPassword)
        {
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.CheckLoginDuplicate(It.Is<string>(l => l.Length >= 5 &&
                                                                     !string.IsNullOrEmpty(l)))).
                                                                     Returns(true).Verifiable();
            UserController controller = new(mock.Object);

            var result = controller.CheckRegister(first, second, login, password, repPassword);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual(newResult.ActionName, "SignIn");
            mock.VerifyAll();
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin", "12345678", "12345678")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "qweasdzxc")]
        [DataRow("Frank", "Right", "right1", "123qwe123", "123qwe123")]
        public void RegisterUnsuccess(string first, string second, string login,
                                      string password, string repPassword)
        {
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.CheckLoginDuplicate(It.Is<string>(l => l.Length >= 5 &&
                                                                   !string.IsNullOrEmpty(l)))).Returns(false);
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
        }

        [TestMethod]
        [DataRow("Comment text", "admin")]
        [DataRow("Test text", "right")]
        [DataRow("Comment", "frank")]
        public void AddComment(string commentText, string ownerLogin)
        {
            User user = new("admin", "secondName", ownerLogin, "123123");
            Mock<IUserAction> mock = new();
            mock.Setup(m => m.FindUser(It.Is<string>(s => s.Length >= 5))).Returns(user).Verifiable();
            mock.Setup(m => m.FindPost(It.Is<long>(id => id == 0))).Returns(new Post()).Verifiable();
            mock.Setup(m => m.AddComment(It.Is<Comment>(c => c.Post != null &&
                                                             c.Post.Author.Login == ownerLogin &&
                                                             c.Author != null &&
                                                             c.Text.Length > 5))).Verifiable();
            UserController controller = new(mock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.AddComment(commentText, 0, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
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
            mock.Setup(m => m.FindUser(It.Is<string>(s => s.Length >= 5))).Returns(user).Verifiable();
            mock.Setup(m => m.FindPost(It.Is<long>(id => id == 0))).Returns(new Post()).Verifiable();
            mock.Setup(m => m.AddComment(It.Is<Comment>(c => c.Post != null &&
                                                             c.Author != null &&
                                                             c.Text.Length > 5))).Verifiable();
            UserController controller = new(mock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = controller.AddComment(commentText, 0, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("VisitUserPage", redirect.ActionName);
            mock.VerifyAll();
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
            mock.Setup(m => m.FindUser(It.IsAny<string>())).Returns(user).Verifiable();
            mock.Setup(m => m.ReturnUserPost(It.Is<User>(u => u.FirstName == first &&
                                                            u.SecondName == second &&
                                                            u.Login == login))).Returns(new List<Post>()).Verifiable();
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
            mock.Setup(m => m.FindUser(It.Is<string>(s => s.Length >= 5))).Returns(user).Verifiable();
            mock.Setup(m => m.AddPost(It.Is<Post>(p => p.Text.Length > 5 && p.Author != null))).Verifiable();
            UserController controller = new(mock.Object);

            var result = controller.AddPost(title, postText, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", redirect.ActionName);
            mock.Verify();
        }
    }
}
