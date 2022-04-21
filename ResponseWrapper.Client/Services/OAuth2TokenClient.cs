using Newtonsoft.Json;
using ResponseWrapper.Client.DI;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace ResponseWrapper.Client.Services
{
    public interface IOAuth2TokenClient
    {
        TokenResponse GetNewToken();
        TokenResponse GetNewUserToken();
    }

    public class OAuth2TokenClient : IOAuth2TokenClient
    {
        readonly Oauth2TokenConfig _oauth2TokenConfig;

        public OAuth2TokenClient(Oauth2TokenConfig oauth2TokenConfig)
        {
            _oauth2TokenConfig = oauth2TokenConfig;
        }

        public TokenResponse GetNewToken()
        {
            try
            {
                var applicationTokenConfig = _oauth2TokenConfig.ApplicationTokenConfig;

                using (var client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                    var data = new NameValueCollection();

                    data.Add("client_id", applicationTokenConfig.ClientId);
                    data.Add("username", applicationTokenConfig.UserName);
                    data.Add("password", applicationTokenConfig.Password);
                    data.Add("grant_type", applicationTokenConfig.GrantType);//"password"
                    data.Add("scope", applicationTokenConfig.Scope);//"openid"

                    // "https://psghubnp.psg.co.za/adfs/oauth2/token"
                    var response = client.UploadValues(_oauth2TokenConfig.OauthUrl, data);

                    var responseString = System.Text.UTF8Encoding.UTF8.GetString(response);

                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseString);

                    return tokenResponse;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    throw;
                }
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                throw new System.Exception(resp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public TokenResponse GetNewUserToken()
        {
            try
            {
                var applicationTokenConfig = _oauth2TokenConfig.UserTokenConfig;

                using (var client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                    var data = new NameValueCollection();

                    data.Add("client_id", applicationTokenConfig.ClientId);
                    data.Add("username", applicationTokenConfig.UserName);
                    data.Add("password", applicationTokenConfig.Password);
                    data.Add("grant_type", applicationTokenConfig.GrantType);//"password"
                    data.Add("scope", applicationTokenConfig.Scope);//"openid email profile"

                    // "https://psghubnp.psg.co.za/adfs/oauth2/token"
                    var response = client.UploadValues(_oauth2TokenConfig.OauthUrl, data);

                    var responseString = System.Text.UTF8Encoding.UTF8.GetString(response);

                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseString);

                    return tokenResponse;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    throw;
                }
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                throw new System.Exception(resp);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
