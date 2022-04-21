using System;

namespace ResponseWrapper.Client.Services
{
    public class Oauth2TokenDataProvider : ITokenDataProvider
    {
        readonly IOAuth2TokenClient _oAuth2TokenClient;

        public Oauth2TokenDataProvider(IOAuth2TokenClient oAuth2TokenClient)
        {
            _oAuth2TokenClient = oAuth2TokenClient;
        }

        TokenInfo _appToken = new TokenInfo();

        TokenInfo _userToken = new TokenInfo();

        public string GetToken()
        {
            if (ShouldRefreshToken(_appToken))
            {
                var token = _oAuth2TokenClient.GetNewToken();

                _appToken = new TokenInfo
                {
                    Token = token.access_token,
                    Expires = DateTime.Now.AddSeconds(token.expires_in)
                };
            }

            return _appToken.Token;
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
