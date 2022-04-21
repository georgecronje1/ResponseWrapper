using System;
using System.Linq;
using ResponseWrapper.Client.Models;

namespace ResponseWrapper.Client.DI
{
    public class ResponseWrapperConfig
    {
        public TokenProvider TokenProviderType { get; private set; } = TokenProvider.None;

        public Oauth2TokenConfig Oauth2TokenConfig { get; private set; } = null;

        public Func<GenericErrorResult, Exception> GenericRestErrorHandler { get; private set; } = ResponseWrapperConfig.HandleErrorResponse;

        public Func<StandardResponse<object>, Exception> ResponseWrapperRestErrorHandler { get; private set; } = ResponseWrapperConfig.HandleResponseWrapperException;

        public void UseDefaultHeaderProvider()
        {
            TokenProviderType = TokenProvider.DefaultHeaderProvider;
        }

        public void UseCustomHeaderProvider()
        {
            TokenProviderType = TokenProvider.CustomHeaderProvider;
        }

        public void UseDefaultOAuth2TokenProvider(Oauth2TokenConfig oauth2TokenConfig)
        {
            TokenProviderType = TokenProvider.DefaultOauth2Provider;
            Oauth2TokenConfig = oauth2TokenConfig;
        }

        public void UseAutoGenUserTokenProvider(AuthGenUserSettings authGenUserSettings)
        {
            TokenProviderType = TokenProvider.AllowAutoGenUserToken;
            Oauth2TokenConfig = Oauth2TokenConfig.MakeForAutoGenUserToken(authGenUserSettings.OAuthURL, authGenUserSettings.UserTokenSettings);
        }

        [Obsolete]
        public void SetGenericRestErrorHandler(Func<GenericErrorResult, Exception> genericRestErrorHandler)
        {
            if (genericRestErrorHandler != null)
            {
                GenericRestErrorHandler = genericRestErrorHandler;
            }
        }

        [Obsolete]
        public void SetGenericRestErrorHandler( Func<StandardResponse<object>, Exception> responseWrapperErrorHandler)
        {
            if (responseWrapperErrorHandler != null)
            {
                ResponseWrapperRestErrorHandler = responseWrapperErrorHandler;
            }
        }

        static Exception HandleErrorResponse(GenericErrorResult error)
        {
            var ex = new Exception();

            ex.Data.Add("StatusCode", error.StatusCode);
            ex.Data.Add("Uri", error.Uri);
            if (error.ErrorResult != null)
            {
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(error.ErrorResult);
                ex.Data.Add("ErrorResult", jsonString);
            }

            return ex;
        }

        static Exception HandleResponseWrapperException(StandardResponse<object> response)
        {
            var ex = new Exception(response.Message);

            ex.Data.Add("ResponseType", response.ResponseType);

            if (response.Error != null)
            {
                if (string.IsNullOrWhiteSpace(response.Error.ErrorMessage) == false)
                {
                    ex.Data.Add("ErrorMessage", response.Error.ErrorMessage);
                }

                if (response.Error.ErrorResult != null)
                {
                    ex.Data.Add("ErrorResult", Newtonsoft.Json.JsonConvert.SerializeObject(response.Error.ErrorResult));
                }
            }

            if (response.ValidationErrors != null && response.ValidationErrors.Any())
            {
                response.ValidationErrors.ToList().ForEach(e => ex.Data.Add($"ValidationError.{e.Key}", e.Message));
            }

            return ex;
        }
    }

    public enum TokenProvider
    {
        None,
        DefaultHeaderProvider,
        DefaultOauth2Provider,
        CustomHeaderProvider,
        AllowAutoGenUserToken
    }

    public class Oauth2TokenConfig
    {
        public string OauthUrl { get; }

        public ApplicationTokenSettings ApplicationTokenConfig { get; }

        public ApplicationTokenSettings UserTokenConfig { get; }

        public class ApplicationTokenSettings
        {
            public string UserName { get; }

            public string Password { get; }

            public string ClientId { get; }

            public string GrantType { get; } = "password";

            public string[] Scopes { get; } = new string[] { "openid" };

            internal string Scope
            {
                get
                {
                    return string.Join(' ', Scopes);
                }
            }

            ApplicationTokenSettings(string userName, string password, string clientId, string grantType, string[] scopes)
            {
                UserName = userName;
                Password = password;
                ClientId = clientId;
                GrantType = grantType;
                Scopes = scopes;
            }

            public static ApplicationTokenSettings Make(string userName, string password, string clientId, string grantType, string[] scopes)
            {
                return new ApplicationTokenSettings(userName, password, clientId, grantType, scopes);
            }

            public static ApplicationTokenSettings MakeDefault(string userName, string password, string clientId)
            {
                return Make(userName, password, clientId, "password", new string[] { "openid" });
            }

            public static ApplicationTokenSettings MakeDefaultWithScopes(string userName, string password, string clientId, string[] scopes)
            {
                return Make(userName, password, clientId, "password", scopes);
            }
        }

        Oauth2TokenConfig(string oauthUrl, ApplicationTokenSettings applicationTokenConfig, ApplicationTokenSettings userTokenConfig)
        {
            OauthUrl = oauthUrl;
            ApplicationTokenConfig = applicationTokenConfig;
            UserTokenConfig = userTokenConfig;
        }

        public static Oauth2TokenConfig Make(string oauthUrl, ApplicationTokenSettings applicationTokenConfig, ApplicationTokenSettings userTokenConfig)
        {
            return new Oauth2TokenConfig(oauthUrl, applicationTokenConfig, userTokenConfig);
        }

        public static Oauth2TokenConfig MakeForAutoGenUserToken(string oauthUrl, ApplicationTokenSettings userTokenConfig)
        {
            return new Oauth2TokenConfig(oauthUrl, null, userTokenConfig);
        }
    }
}
