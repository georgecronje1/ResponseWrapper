using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace ResponseWrapper.Client.Services
{
    public class AutoGenUserTokenProvider : ITokenDataProvider
    {
        readonly IOAuth2TokenClient _oAuth2TokenClient;
        readonly IHttpContextAccessor _httpContextAccessor;

        public AutoGenUserTokenProvider(IOAuth2TokenClient oAuth2TokenClient, IHttpContextAccessor httpContextAccessor)
        {
            _oAuth2TokenClient = oAuth2TokenClient;
            _httpContextAccessor = httpContextAccessor;
        }

        TokenInfo _userToken = new TokenInfo();

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

        bool ShouldRefreshToken(TokenInfo tokenInfo)
        {
            if (tokenInfo == null)
            {
                return true;
            }

            bool doNotHaveValues = string.IsNullOrWhiteSpace(tokenInfo.Token) || tokenInfo.Expires.HasValue == false;
            if (doNotHaveValues)
            {
                return true;
            }

            bool aboutToExpire = DateTime.Now >= tokenInfo.Expires.Value.AddMinutes(-1);

            return doNotHaveValues || aboutToExpire;
        }

        public string GetUserToken()
        {
            StringValues token = string.Empty;
            var getTokenResult = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("x-api-token", out token);
            if (getTokenResult)
            {
                return token;
            }

            if (ShouldRefreshToken(_userToken))
            {
                var proxyToken = _oAuth2TokenClient.GetNewUserToken();
                string accessToken = string.Empty;
                try
                {
                    accessToken = ProxyTokenHandler.DecodeProxyToken(proxyToken.access_token);
                }
                catch
                {
                    accessToken = proxyToken.access_token;
                }

                _userToken = new TokenInfo
                {
                    Token = accessToken,
                    Expires = DateTime.Now.AddSeconds(proxyToken.expires_in)
                };
            }

            return _userToken.Token;
        }
    }
}
