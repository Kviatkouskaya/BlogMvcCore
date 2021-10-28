using BlogMvcCore.Controllers;
using BlogMvcCore.DomainModel;
using BlogMvcCore.Helpers;
using BlogMvcCore.Models;
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
            RepositoryInMemory repository = new();
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
            RepositoryInMemory repository = new();
            UserController controller = new(repository);

            var result = controller.CheckRegister(first, second, login, password, repPassword);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("Register", newResult.ActionName);
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
        public void VisitUserPageTest(string first, string second, string login, string password)
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
        [DataRow("Admin", "System", "admin1", "12345678", "fakeLogin")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "")]
        [DataRow("Frank", "Right", "right1", "123qwe123", null)]
        public void VisitUserPageUnsuccessTest(string first, string second, string login,
                                               string password, string fakeLogin)
        {
            Mock<IUserAction> mock = new();
            UserController controller = new(mock.Object);
            MockHttpSession mockHttpSession = new();
            controller.ControllerContext = CreateControllerContext(mockHttpSession);
            User user = new(first, second, login, password);
            controller.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);
            mock.Setup(m => m.FindUser(login)).Returns(user);
            mock.Setup(m => m.ReturnUserPost(user)).Returns(new List<Post>());

            var result = controller.VisitUserPage(fakeLogin);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        //rewriting

        [TestMethod]
        [DataRow("Post title", "Post text", "admin")]
        [DataRow("New Post title", "New post text", "adamsm")]
        [DataRow("Test title", "Test post text", "right")]
        public void AddPostCheck(string title, string postText, string ownerLogin)
        {
            RepositoryInMemory repository = new();
            UserController controller = new(repository);

            var result = controller.AddPost(title, postText, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var postAuthor = repository.allowedUsers.Find(u => u.Login == ownerLogin);
            var postResult = postAuthor.Posts.Find(p => p.Title == title && p.Text == postText);
            Assert.IsNotNull(postResult);
            Assert.IsInstanceOfType(postResult, typeof(Post));
            Assert.IsTrue(postResult.Title == title && postResult.Text == postText
                                                    && postResult.Author.Login == ownerLogin);
        }

        [TestMethod]
        [DataRow("Post title", "", "admin")]
        [DataRow("", "Test post text", "right")]
        public void AddPostUnsuccess(string title, string postText, string ownerLogin)
        {
            RepositoryInMemory repository = new();
            UserController controller = new(repository);

            var result = controller.AddPost(title, postText, ownerLogin);

            Assert.IsInstanceOfType(result, typeof(IActionResult));
            var postAuthor = repository.allowedUsers.Find(u => u.Login == ownerLogin);
            var resultPost = postAuthor.Posts?.Find(p => p.Text == postText && p.Title == title);
            Assert.IsNull(resultPost);
        }
    }
}
