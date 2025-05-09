﻿namespace Octavus.Authentication
{
    public class KeycloakOptions
    {
        public string Authority { get; set; }
        public string Realm { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
