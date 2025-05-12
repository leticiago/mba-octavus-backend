using Octavus.Core.Application.DTO;

namespace Octavus.Core.Application.Services
{
    public interface IKeycloakService
    {
        Task<string?> AuthenticateAsync(string username, string password);
        Task<bool> CreateUserAndAssignRolesAsync(KeycloakUser user);
        Task<bool> CreateUserAsync(KeycloakUser user, string token);
        Task<string?> GetUserIdByUsernameAsync(string username, string token);
        Task<bool> AssignRolesToUserAsync(string userId, IEnumerable<string> roles, string token);
        Task<bool> LogoutAsync(string accessToken);
    }

}
