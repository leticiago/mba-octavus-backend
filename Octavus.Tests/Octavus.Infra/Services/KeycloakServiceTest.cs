using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework;
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
            keycloakSection.Setup(s => s["auth-server-url"]).Returns("https://keycloak.test/");

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
        public async Task AuthenticateAsync_ReturnsAccessToken_WhenResponseIsSuccess()
        {
            // Arrange
            var tokenJson = "{\"access_token\":\"abc123token\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenJson)
            };

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await _service.AuthenticateAsync("user", "password");

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo("abc123token"));
        }

        [Test]
        public async Task AuthenticateAsync_ReturnsNull_WhenResponseIsFailure()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await _service.AuthenticateAsync("user", "password");

            // Assert
            Assert.IsNull(result);
        }
    }
}
