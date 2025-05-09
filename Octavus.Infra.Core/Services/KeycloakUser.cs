using System.Collections.Generic;

namespace Octavus.Infra.Core.Services
{
    public class KeycloakUser
    {
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public List<Credential> Credentials { get; set; } = new();
        public List<string> Roles { get; set; } = new();

        public class Credential
        {
            public string Type { get; set; } = "password";
            public string Value { get; set; } = string.Empty;
            public bool Temporary { get; set; } = false;
        }
    }
}
