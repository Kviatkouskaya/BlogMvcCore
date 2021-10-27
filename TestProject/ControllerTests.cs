using BlogMvcCore.Controllers;
using BlogMvcCore.DomainModel;
using BlogMvcCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class ControllerTests
    {
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

        [TestMethod]
        [DataRow("Admin", "System", "admin1", "12345678", "12345678")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "qweasdzxc")]
        [DataRow("Frank", "Right", "right1", "123qwe123", "123qwe123")]
        public void CheckRegisterTest(string first, string second, string login,
                                      string password, string repPassword)
        {
            //arrange
            RepositoryInMemory repository = new();
            UserController controller = new(repository);
            //act
            var result = controller.CheckRegister(first, second, login, password, repPassword);
            //assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual(newResult.ActionName, "SignIn");
            User resultChecks = repository.allowedUsers.Find(u => u.Login == login);
            Assert.AreEqual(login, resultChecks.Login);
        }

        [TestMethod]
        [DataRow("Admin", "System", null, "12345678", "12345678")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "qweasdzx11")]
        [DataRow("Frank", "Right", "right1", "", "123qwe123")]
        public void CheckRegisterUnsuccessTest(string first, string second, string login,
                                               string password, string repPassword)
        {
            RepositoryInMemory repository = new();
            UserController controller = new(repository);

            var result = controller.CheckRegister(first, second, login, password, repPassword);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual(newResult.ActionName, "Register");
            User resultChecks = repository.allowedUsers.Find(u => u.Login == login);
            Assert.IsNull(resultChecks);
        }

        [TestMethod]
        [DataRow("admin", "12345678")]
        [DataRow("adamsm", "qweasdzxc")]
        [DataRow("right", "123qwe123")]
        public void CheckInSuccessTest(string login, string password)
        {
            MockHttpSession mockHttpSession = new();
            RepositoryInMemory repository = new();
            UserController controller = new(repository);
            controller.ControllerContext = CreateControllerContext(mockHttpSession);

            var result = controller.CheckIn(login, password);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual(newResult.ActionName, "UserPage");
            var sessionResult = mockHttpSession["user"];
            Assert.IsNotNull(sessionResult);
        }

        [TestMethod]
        [DataRow("admin", "12345679")]
        [DataRow("", "qweasdzxc")]
        [DataRow("right", null)]
        public void CheckInUnsuccessTest(string login, string password)
        {
            RepositoryInMemory repository = new();
            UserController controller = new(repository);

            var result = controller.CheckIn(login, password);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual(newResult.ActionName, "SignIn");
        }
        private static ControllerContext CreateControllerContext(MockHttpSession mockHttpSession)
        {
            DefaultHttpContext httpContext = new();
            httpContext.Session = mockHttpSession;
            ControllerContext controllerContext = new();
            controllerContext.HttpContext = httpContext;
            return controllerContext;
        }
    }
}
