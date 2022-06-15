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
        /*
        [TestMethod]
        [DataRow("Admin", "System", "admin", "12345678")]
        public void CheckIn(string first, string second, string login, string password)
        {

            User user = new(first, second, login, password);
            AuthMock.Setup(x => x.CheckIn(login, password)).Returns(user).Verifiable();
            MockHttpSession mockHttpSession = new();
            UserController userController = new(AuthMock.Object, UserServiceMock.Object, PostServiceMock.Object, CommentServiceMock.Object);
            userController.ControllerContext = CreateControllerContext(mockHttpSession);

            var result = userController.CheckIn(login, password);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", newResult.ActionName);
            var sessionResult = mockHttpSession.GetString("user");
            Assert.IsNotNull(sessionResult);
            var userSession = JsonConvert.DeserializeObject<User>(sessionResult.ToString());
            Assert.AreEqual(user, userSession);
            AuthMock.VerifyAll();
        }

        [TestMethod]
        [DataRow("admin", "12345679", null)]
        [DataRow("adams1", "qweasdzxc", null)]
        [DataRow("ight1", "123qwe123", null)]
        public void CheckInFail(string login, string password, User user)
        {
            AuthMock.Setup(x => x.CheckIn(login, password)).Returns(user).Verifiable();
            MockHttpSession mockHttpSession = new();
            UserController userController = new(AuthMock.Object, UserServiceMock.Object, PostServiceMock.Object, CommentServiceMock.Object);
            userController.ControllerContext = CreateControllerContext(mockHttpSession);

            var result = userController.CheckIn(login, password);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("SignIn", newResult.ActionName);
            AuthMock.VerifyAll();
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin1", "12345678", "12345678")]
        [DataRow("Adam", "Smith", "adamsm1", "qweasdzxc", "qweasdzxc")]
        [DataRow("Frank", "Right", "right1", "123qwe123", "123qwe123")]
        public void Register(string first, string second, string login,
                             string password, string repPassword)
        {
            AuthMock.Setup(x => x.CheckUserRegistration(first, second, login, password, repPassword)).Returns(true).Verifiable();
            UserController userController = new(AuthMock.Object, UserServiceMock.Object,
                                                PostServiceMock.Object, CommentServiceMock.Object);

            var result = userController.CheckRegister(first, second, login, password, repPassword);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual(newResult.ActionName, "SignIn");
            AuthMock.VerifyAll();
        }

        [TestMethod]
        [DataRow("Admin", "System", "", "12345678", "12345678")]
        [DataRow("Adam", "", "adamsm1", "qweasdzxc", "qweasdzxc")]
        [DataRow("Frank", "Right", "right1", "123qwe123", "")]
        public void RegisterFail(string first, string second, string login,
                                 string password, string repPassword)
        {
            AuthMock.Setup(x => x.CheckUserRegistration(first, second, login, password, repPassword)).Returns(false).Verifiable();
            UserController userController = new(AuthMock.Object, UserServiceMock.Object,
                                                PostServiceMock.Object, CommentServiceMock.Object);

            var result = userController.CheckRegister(first, second, login, password, repPassword);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("Register", newResult.ActionName);
            AuthMock.VerifyAll();
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
        [DataRow("Comment text", "admin", 1, 21)]
        [DataRow("Test text", "right", 3, 2)]
        [DataRow("Comment", "frank", 4, 5)]
        public void AddComment(string commentText, string ownerLogin, long postID, long parentID)
        {
            User user = new("admin", "secondName", ownerLogin, "123123");
            UserController userController = new(AuthMock.Object, UserServiceMock.Object,
                                                PostServiceMock.Object, CommentServiceMock.Object);
            AuthMock.Setup(x => x.CheckStringParams(commentText)).Returns(true).Verifiable();
            CommentServiceMock.Setup(x => x.AddComment(commentText, postID, parentID, user)).Verifiable();
            MockHttpSession mockHttpSession = new();
            userController.ControllerContext = CreateControllerContext(mockHttpSession);
            userController.ControllerContext.HttpContext.Session.SetUserAsJson("user", user);

            var result = userController.AddComment(commentText, postID, parentID);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.AreEqual("ViewPostAndComments", redirect.ActionName);
            AuthMock.VerifyAll();
            CommentServiceMock.VerifyAll();
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
        */
    }
}
