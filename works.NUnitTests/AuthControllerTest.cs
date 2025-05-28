using System.Security.Claims;
using AuthAPI.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Moq;

namespace AuthAPI.Tests.Controllers
{
    public class AuthControllerTest
    {
        private AuthController GetControllerWithContext(Mock<IAuthenticationService> authServiceMock = null)
        {
            var controller = new AuthController();

            var httpContext = new DefaultHttpContext();
            if (authServiceMock != null)
            {
                httpContext.RequestServices = new ServiceProviderStub(authServiceMock.Object);
            }
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            return controller;
        }

        [Test]
        public void Signin_ReturnsOk_WhenCredentialsAreCorrect()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                "CookieAuth",
                It.IsAny<ClaimsPrincipal>(),
                null)).Returns(Task.CompletedTask);

            var controller = GetControllerWithContext(authServiceMock);

            // Act
            var result = controller.Signin("test", "0000");

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual("登入成功", okResult.Value);
        }

        [Test]
        public void Signin_ReturnsUnauthorized_WhenCredentialsAreIncorrect()
        {
            // Arrange
            var controller = GetControllerWithContext();

            // Act
            var result = controller.Signin("wrong", "creds");

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual("使用者名稱或密碼錯誤", unauthorizedResult.Value);
        }

        [Test]
        public void Signin_ReturnsBadRequest_OnException()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                "CookieAuth",
                It.IsAny<ClaimsPrincipal>(),
                null)).Throws(new System.Exception("fail"));

            var controller = GetControllerWithContext(authServiceMock);

            // Act
            var result = controller.Signin("test", "0000");

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.Value.ToString(), Does.Contain("登入失敗"));
        }

        // Helper stub for DI
        private class ServiceProviderStub : IServiceProvider
        {
            private readonly IAuthenticationService _authService;
            public ServiceProviderStub(IAuthenticationService authService)
            {
                _authService = authService;
            }
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IAuthenticationService))
                    return _authService;
                return null;
            }
        }
    }
}