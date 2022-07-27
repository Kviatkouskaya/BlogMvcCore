using BlogMvcCore.Controllers;
using BlogMvcCore.DomainModel;
using BlogMvcCore.Storage;
using BlogMvcCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using BlogMvcCore.Helpers;

namespace TestProject.Controller_Tests
{
    [TestClass]
    public class AuthenticationTests
    {
        private static IAuthenticationRepository AuthenticationRepository { get; set; }
        private static IUserRepository UserRepository { get; set; }
        private readonly Mock<AuthenticationService> AuthMock = new(AuthenticationRepository, UserRepository);

        private static ControllerContext CreateControllerContext(MockHttpSession mockHttpSession)
        {
            DefaultHttpContext httpContext = new();
            httpContext.Session = mockHttpSession;
            ControllerContext controllerContext = new();
            controllerContext.HttpContext = httpContext;
            return controllerContext;
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin", "12345678")]
        public void SignOut(string first, string second, string login, string password)
        {
            UserDomain user = new(first, second, login, password);
            AuthMock.Setup(x => x.SignOut()).Verifiable();
            MockHttpSession mockHttpSession = new();
            mockHttpSession.SetUserAsJson("user", user);
            AuthenticationController controller = new(AuthMock.Object);
            controller.ControllerContext = CreateControllerContext(mockHttpSession);

            var result = controller.SignOut();

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", newResult.ActionName);
            var sessionResult = mockHttpSession.GetString("user");
            var userSession = JsonConvert.DeserializeObject<UserDomain>(sessionResult.ToString());
            Assert.AreEqual(null, userSession);
            AuthMock.VerifyAll();
        }

        [TestMethod]
        [DataRow("Admin", "System", "admin", "12345678")]
        public void CheckIn(string first, string second, string login, string password)
        {

            UserDomain user = new(first, second, login, password);
            AuthMock.Setup(x => x.CheckIn(login, password)).Returns(user).Verifiable();
            MockHttpSession mockHttpSession = new();
            AuthenticationController controller = new(AuthMock.Object);
            controller.ControllerContext = CreateControllerContext(mockHttpSession);

            var result = controller.CheckIn(login, password);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("UserPage", newResult.ActionName);
            var sessionResult = mockHttpSession.GetString("user");
            AuthMock.VerifyAll();
        }

        [TestMethod]
        [DataRow("admin", "12345679", null)]
        [DataRow("adams1", "qweasdzxc", null)]
        [DataRow("ight1", "123qwe123", null)]
        public void CheckInFail(string login, string password, UserDomain user)
        {
            AuthMock.Setup(x => x.CheckIn(login, password)).Returns(user).Verifiable();
            MockHttpSession mockHttpSession = new();
            AuthenticationController userController = new(AuthMock.Object);
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
            AuthMock.Setup(x => x.AddUser(first, second, login, password)).Returns(true).Verifiable();
            AuthenticationController userController = new(AuthMock.Object);

            var result = userController.CheckRegister(first, second, login, password);

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
            AuthMock.Setup(x => x.AddUser(first, second, login, password)).Returns(false).Verifiable();
            AuthenticationController userController = new(AuthMock.Object);

            var result = userController.CheckRegister(first, second, login, password);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult newResult = (RedirectToActionResult)result;
            Assert.AreEqual("Register", newResult.ActionName);
            AuthMock.VerifyAll();
        }
    }
}
