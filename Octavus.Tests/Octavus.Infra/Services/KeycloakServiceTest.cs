using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Octavus.Core.Application.DTO;
using Octavus.Infra.Core.Services;

namespace Octavus.Tests.Services
{
    public class KeycloakServiceTests
    {
        private Mock<IConfiguration> _mockConfig = null!;
        private Mock<HttpMessageHandler> _mockHttpHandler = null!;
        private HttpClient _httpClient = null!;
        private KeycloakService _service = null!;
        [SetUp]
        public void Setup()
        {
            _mockConfig = new Mock<IConfiguration>();

            var keycloakSection = new Mock<IConfigurationSection>();
            keycloakSection.Setup(s => s["resource"]).Returns("client-id");
            keycloakSection.Setup(s => s["realm"]).Returns("realm-name");
            keycloakSection.Setup(s => s["credentials:secret"]).Returns("secret");

            keycloakSection.Setup(s => s["auth-server-url"]).Returns("https://keycloak.test");

            _mockConfig.Setup(c => c.GetSection("Keycloak")).Returns(keycloakSection.Object);

            _mockHttpHandler = new Mock<HttpMessageHandler>(MockBehavior.Loose);

            _httpClient = new HttpClient(_mockHttpHandler.Object);

            _service = new KeycloakService(_httpClient, _mockConfig.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _httpClient.Dispose();
        }

        [Test]
        public async Task CreateUserAsync_ReturnsTrue_WhenResponseIsSuccess()
        {
            var token = "valid-token";
            var user = new KeycloakUser
            {
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Enabled = true,
                Credentials = new List<KeycloakUser.Credential>()
            };

            var response = new HttpResponseMessage(HttpStatusCode.Created);

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.ToString().Contains("/users")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _service.CreateUserAsync(user, token);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task CreateUserAsync_ReturnsFalse_WhenResponseIsFailure()
        {
            var token = "valid-token";
            var user = new KeycloakUser
            {
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Enabled = true,
                Credentials = new List<KeycloakUser.Credential>()
            };

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error details")
            };

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.ToString().Contains("/users")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _service.CreateUserAsync(user, token);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AuthenticateAsync_ReturnsToken_WhenResponseIsSuccess()
        {
            var tokenJson = "{\"access_token\":\"valid-access-token\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenJson)
            };

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.ToString().Contains("/protocol/openid-connect/token")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var token = await _service.AuthenticateAsync("user", "pass");

            Assert.That(token, Is.EqualTo("valid-access-token"));
        }

        [Test]
        public async Task AuthenticateAsync_ReturnsNull_WhenResponseIsFailure()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var token = await _service.AuthenticateAsync("user", "pass");

            Assert.IsNull(token);
        }

        [Test]
        public async Task GetUserIdByUsernameAsync_ReturnsUserId_WhenUserExists()
        {
            var userId = "12345";
            var userJson = $"[{{\"id\":\"{userId}\",\"username\":\"testuser\"}}]";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(userJson)
            };

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _service.GetUserIdByUsernameAsync("testuser", "token");

            Assert.That(result, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetUserIdByUsernameAsync_ReturnsNull_WhenUserNotFound()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            };

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _service.GetUserIdByUsernameAsync("nonexistent", "token");

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetUserIdByUsernameAsync_ReturnsNull_WhenResponseIsFailure()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _service.GetUserIdByUsernameAsync("anyuser", "token");

            Assert.IsNull(result);
        }

        [Test]
        public async Task AssignRolesToUserAsync_ReturnsTrue_WhenAllRolesAssigned()
        {
            var userId = "user123";
            var roles = new[] { "role1", "role2" };

            foreach (var role in roles)
            {
                // Mock GET role
                var roleJson = $"{{\"id\":\"role-id-{role}\",\"name\":\"{role}\",\"composite\":false,\"clientRole\":false,\"containerId\":\"realm\"}}";
                var getRoleResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(roleJson)
                };
                _mockHttpHandler.Protected()
                    .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.ToString().Contains($"/roles/{role}")),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(getRoleResponse);

                // Mock POST assign role
                var assignResponse = new HttpResponseMessage(HttpStatusCode.NoContent);
                _mockHttpHandler.Protected()
                    .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri!.ToString().Contains($"/users/{userId}/role-mappings/realm")),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(assignResponse);
            }

            var result = await _service.AssignRolesToUserAsync(userId, roles, "token");

            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetAdminAccessTokenAsync_ReturnsToken_WhenSuccess()
        {
            var tokenJson = "{\"access_token\":\"admin-token\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenJson)
            };

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Chamando método privado via reflexão para teste (ou alterar para internal + InternalsVisibleTo)
            var method = typeof(KeycloakService).GetMethod("GetAdminAccessTokenAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (Task<string?>)method!.Invoke(_service, null)!;
            var token = await task;

            Assert.That(token, Is.EqualTo("admin-token"));
        }

        [Test]
        public async Task GetAdminAccessTokenAsync_ReturnsNull_WhenFailure()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var method = typeof(KeycloakService).GetMethod("GetAdminAccessTokenAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (Task<string?>)method!.Invoke(_service, null)!;
            var token = await task;

            Assert.IsNull(token);
        }

        [Test]
        public async Task LogoutAsync_ReturnsTrue_WhenSuccess()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _service.LogoutAsync("some-token");

            Assert.IsTrue(result);
        }

        [Test]
        public async Task LogoutAsync_ReturnsFalse_WhenFailure()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _service.LogoutAsync("some-token");

            Assert.IsFalse(result);
        }


    }
}
