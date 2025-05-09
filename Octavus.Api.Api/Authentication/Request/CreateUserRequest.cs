using System.Collections.Generic;

namespace Octavus.App.Api.Authentication.Request
{
    public class CreateUserRequest
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public bool Enabled { get; set; } = true;

        public List<Credential> Credentials { get; set; } = new();

        public class Credential
        {
            public string Type { get; set; } = "password";
            public string Value { get; set; } = default!;
            public bool Temporary { get; set; } = false;
        }
    }
}
