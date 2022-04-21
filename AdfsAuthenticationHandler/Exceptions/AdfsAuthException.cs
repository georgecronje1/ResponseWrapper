using System;

namespace AdfsAuthenticationHandler.Exceptions
{
    public class AdfsAuthException : Exception
    {
        public AdfsAuthException(string message) : base($"Authentication Failed: {message}") { }
    }
}
