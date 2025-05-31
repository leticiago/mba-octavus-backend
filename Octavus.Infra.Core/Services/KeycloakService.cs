using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Net.Http;
using Octavus.Infra.Core;
using Octavus.Infra.Core.Services;
using System.Data;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;

namespace Octavus.Infra.Core.Services
{
    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public JwtSecurityToken? Token { get; set; }
    }

    public class KeycloakService : IKeycloakService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public KeycloakService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            var baseUrl = _configuration.GetSection("Keycloak")["auth-server-url"]?.TrimEnd('/');
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var keycloakSection = _configuration.GetSection("Keycloak");
            var clientId = keycloakSection["resource"];
            var realm = keycloakSection["realm"];
            var clientSecret = keycloakSection["credentials:secret"];
            var url = $"{keycloakSection["auth-server-url"]}/realms/{realm}/protocol/openid-connect/token";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "password",
                    ["client_id"] = clientId,
                    ["username"] = username,
                    ["password"] = password,
                    ["client_secret"] = clientSecret
                })
            };

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            return token?["access_token"].GetString();
        }

        public async Task<bool> CreateUserAndAssignRolesAsync(KeycloakUser user)
        {
            var token = await GetAdminAccessTokenAsync();
            if (token == null) return false;

            var created = await CreateUserAsync(user, token);
            if (!created) return false;

            var userId = await GetUserIdByUsernameAsync(user.Username, token);
            if (userId == null) return false;

            return await AssignRolesToUserAsync(userId, user.Roles, token);
        }

        public async Task<bool> CreateUserAsync(KeycloakUser user, string token)
        {
            var realm = _configuration["Keycloak:realm"];
            var baseUrl = _configuration["Keycloak:auth-server-url"];
            var createUserUrl = $"{baseUrl}admin/realms/{realm}/users";

            var userPayload = new
            {
                username = user.Username,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                enabled = user.Enabled,
                credentials = user.Credentials
            };

            var request = new HttpRequestMessage(HttpMethod.Post, createUserUrl)
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) },
                Content = new StringContent(JsonSerializer.Serialize(userPayload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao criar usuário: {response.StatusCode} - {errorBody}");
                return false;
            }

            return true;
        }

        public async Task<string?> GetUserIdByUsernameAsync(string username, string token)
        {
            var realm = _configuration["Keycloak:realm"];
            var baseUrl = _configuration["Keycloak:auth-server-url"];
            var getUserUrl = $"{baseUrl}admin/realms/{realm}/users?username={username}";

            var request = new HttpRequestMessage(HttpMethod.Get, getUserUrl)
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
            };

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Erro ao buscar usuário: {response.StatusCode} - {body}");
                return null;
            }

            var users = JsonSerializer.Deserialize<List<JsonElement>>(body);
            var user = users?.FirstOrDefault();

            if (user.Value.ValueKind == JsonValueKind.Undefined || user.Value.ValueKind == JsonValueKind.Null)
            {
                Console.WriteLine("Usuário não encontrado.");
                return null;
            }

            return user.Value.GetProperty("id").GetString();
        }

        public async Task<bool> AssignRolesToUserAsync(string userId, IEnumerable<string> roles, string token)
        {
            var realm = _configuration["Keycloak:realm"];
            var baseUrl = _configuration["Keycloak:auth-server-url"];

            foreach (var roleName in roles)
            {
                var getRoleUrl = $"{baseUrl}admin/realms/{realm}/roles/{roleName}";
                var getRoleRequest = new HttpRequestMessage(HttpMethod.Get, getRoleUrl)
                {
                    Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
                };

                var getRoleResponse = await _httpClient.SendAsync(getRoleRequest);
                var getRoleBody = await getRoleResponse.Content.ReadAsStringAsync();

                if (!getRoleResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erro ao buscar role '{roleName}': {getRoleResponse.StatusCode} - {getRoleBody}");
                    return false;
                }

                var role = JsonSerializer.Deserialize<JsonElement>(getRoleBody);
                var roleRepresentation = new[]
                {
            new
            {
                id = role.GetProperty("id").GetString(),
                name = role.GetProperty("name").GetString(),
                composite = role.GetProperty("composite").GetBoolean(),
                clientRole = role.GetProperty("clientRole").GetBoolean(),
                containerId = role.GetProperty("containerId").GetString()
            }
        };

                var assignRoleUrl = $"{baseUrl}admin/realms/{realm}/users/{userId}/role-mappings/realm";
                var assignRequest = new HttpRequestMessage(HttpMethod.Post, assignRoleUrl)
                {
                    Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) },
                    Content = new StringContent(JsonSerializer.Serialize(roleRepresentation), Encoding.UTF8, "application/json")
                };

                var assignResponse = await _httpClient.SendAsync(assignRequest);
                var assignBody = await assignResponse.Content.ReadAsStringAsync();

                if (!assignResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erro ao atribuir role '{roleName}': {assignResponse.StatusCode} - {assignBody}");
                    return false;
                }
            }

            return true;
        }

        private async Task<string?> GetAdminAccessTokenAsync()
        {
            var tokenUrl = $"{_configuration["Keycloak:auth-server-url"]}realms/{_configuration["Keycloak:realm"]}/protocol/openid-connect/token";

            var clientId = _configuration["OctavusAdmin:resource"];
            var clientSecret = _configuration["OctavusAdmin:credentials:secret"];

            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("grant_type", "client_credentials"),
        new KeyValuePair<string, string>("client_id", clientId),
        new KeyValuePair<string, string>("client_secret", clientSecret)
    });

            var response = await _httpClient.PostAsync(tokenUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Token error: {response.StatusCode} - {responseBody}");
                return null;
            }

            var json = JsonSerializer.Deserialize<JsonElement>(responseBody);
            return json.GetProperty("access_token").GetString();
        }
        public async Task<bool> LogoutAsync(string accessToken)
        {
            var keycloakSection = _configuration.GetSection("Keycloak");
            var clientId = keycloakSection["resource"];
            var realm = keycloakSection["realm"];
            var url = $"{keycloakSection["auth-server-url"]}/realms/{realm}/protocol/openid-connect/logout";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["refresh_token"] = accessToken
                })
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

    }

}