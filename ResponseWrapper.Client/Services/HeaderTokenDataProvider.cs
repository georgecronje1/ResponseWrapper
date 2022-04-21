using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace ResponseWrapper.Client.Services
{
    public class HeaderTokenDataProvider : ITokenDataProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeaderTokenDataProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetToken()
        {
            StringValues token = string.Empty;
            var getTokenResult = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out token);
            if (getTokenResult)
            {
                token = token.ToString()
                    .Replace("bearer ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    .Trim();
                return token;
            }
            throw new Exception($"Authorization header not found.");
        }

        public string GetUserToken()
        {
            StringValues token = string.Empty;
            var getTokenResult = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("x-api-token", out token);
            if (getTokenResult)
            {
                return token;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
