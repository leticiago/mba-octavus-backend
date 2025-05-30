using Moq;
using Microsoft.AspNetCore.Mvc;
using Octavus.Authentication.Request;
using Octavus.Infra.Core.Services;
using Microsoft.AspNetCore.Http;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using System.Text.Json;

namespace Octavus.Tests.Controllers
{
    public class AuthenticationControllerTests
    {
        private Mock<IKeycloakService> _keycloakServiceMock;
        private AuthenticationController _controller;

        [SetUp]
        public void Setup()
        {
            _keycloakServiceMock = new Mock<IKeycloakService>();
            _controller = new AuthenticationController(_keycloakServiceMock.Object);
        }

        [Test]
        public async Task CreateUser_ReturnsOk_WhenUserIsCreated()
        {
            // Arrange
            var request = new KeycloakUser();
            _keycloakServiceMock
                .Setup(k => k.CreateUserAndAssignRolesAsync(request))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Usuário criado com sucesso."));
        }

        [Test]
        public async Task CreateUser_ReturnsBadRequest_WhenUserCreationFails()
        {
            var request = new KeycloakUser();
            _keycloakServiceMock
                .Setup(k => k.CreateUserAndAssignRolesAsync(request))
                .ReturnsAsync(false);

            var result = await _controller.CreateUser(request);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.Value, Is.EqualTo("Erro ao criar o usuário."));
        }

        [Test]
        public async Task Login_ReturnsToken_WhenCredentialsAreValid()
        {
            var request = new LoginRequest { Username = "user", Password = "pass" };
            _keycloakServiceMock
                .Setup(k => k.AuthenticateAsync("user", "pass"))
                .ReturnsAsync("fake-jwt-token");

            var result = await _controller.Login(request);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var response = JsonSerializer.Serialize(okResult.Value);
            Assert.IsNotNull(response);
            Assert.That(response, Is.EqualTo("{\"token\":\"fake-jwt-token\"}"));
        }

        [Test]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            var request = new LoginRequest { Username = "user", Password = "wrong" };
            _keycloakServiceMock
                .Setup(k => k.AuthenticateAsync("user", "wrong"))
                .ReturnsAsync((string)null);

            var result = await _controller.Login(request);

            var unauthorized = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorized);
            Assert.That(unauthorized.Value, Is.EqualTo("Credenciais inválidas."));
        }

        [Test]
        public async Task Logout_ReturnsOk_WhenLogoutSucceeds()
        {
            var token = "Bearer fake-token";
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = token;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            _keycloakServiceMock
                .Setup(k => k.LogoutAsync("fake-token"))
                .ReturnsAsync(true);

            var result = await _controller.Logout();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo("Logout realizado com sucesso."));
        }

        [Test]
        public async Task Logout_ReturnsBadRequest_WhenTokenIsMissing()
        {
            var context = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            var result = await _controller.Logout();

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.Value, Is.EqualTo("Token não fornecido."));
        }

        [Test]
        public async Task Logout_ReturnsBadRequest_WhenLogoutFails()
        {
            var token = "Bearer fake-token";
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = token;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            _keycloakServiceMock
                .Setup(k => k.LogoutAsync("fake-token"))
                .ReturnsAsync(false);

            var result = await _controller.Logout();

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.Value, Is.EqualTo("Erro ao efetuar logout."));
        }
    }
}
