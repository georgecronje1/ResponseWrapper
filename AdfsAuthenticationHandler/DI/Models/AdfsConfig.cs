using System;

namespace AdfsAuthenticationHandler.DI.Models
{
    public class AdfsConfig
    {
        public Uri AuthTokenEndpoint { get; private set; }

        public Uri UserAuthorizationEndpoint { get; private set; }

        [Obsolete("This method is deprecated, use SetTokenAuthEndpoints instead.")]
        public void SetTokenEndpoint(string endpoint)
        {
            // Validate here?
            AuthTokenEndpoint = new Uri(endpoint);
        }

        [Obsolete("This method is deprecated, use SetTokenAuthEndpoints instead.")]
        public void SetUserAuthorizationEndpoint(string endpoint)
        {
            UserAuthorizationEndpoint = new Uri(endpoint);
        }

        public void SetTokenAuthEndpoints(string tokenEndpoint, string userAuthEndpoint)
        {
            AuthTokenEndpoint = new Uri(tokenEndpoint);
            UserAuthorizationEndpoint = new Uri(userAuthEndpoint);
        }

    }
}
