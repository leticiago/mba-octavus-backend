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

    }
}
