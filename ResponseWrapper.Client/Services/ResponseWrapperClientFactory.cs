using ResponseWrapper.Client.DI;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;

namespace ResponseWrapper.Client.Services
{
    public interface IResponseWrapperClientFactory
    {
        RestClient GetClient(IResponseWrapperRestClientConfig config);
        RestClient GetClient(IGenericRestClientConfig config);
    }

    public class ResponseWrapperClientFactory : IResponseWrapperClientFactory
    {
        private readonly ITokenDataProvider _tokenDataProvider;
        private readonly ResponseWrapperConfig _responseWrapperConfig;

        public ResponseWrapperClientFactory(ITokenDataProvider tokenDataProvider, ResponseWrapperConfig responseWrapperConfig)
        {
            _tokenDataProvider = tokenDataProvider;
            _responseWrapperConfig = responseWrapperConfig;
        }

        public RestClient GetClient(IResponseWrapperRestClientConfig config)
        {
            var client = new RestClient(config.BaseUrl);
            client.UseNewtonsoftJson();

            if(config.UseAuthHeader && _responseWrapperConfig.TokenProviderType != TokenProvider.None)
            {
                client.AddDefaultHeader("Authorization", $"Bearer {_tokenDataProvider.GetToken()}");
                client.AddDefaultHeader("x-api-token", _tokenDataProvider.GetUserToken());
            }

            // client.Authenticator = new SimpleAuthenticator("username", "foo", "password", "bar");
            // https://restsharp.dev/usage/authenticators.html#using-simpleauthenticator

            return client;
        }

        public RestClient GetClient(IGenericRestClientConfig config)
        {
            var client = new RestClient(config.BaseUrl);
            client.UseNewtonsoftJson();

            if (config.UseAuthHeader && _responseWrapperConfig.TokenProviderType != TokenProvider.None)
            {
                client.AddDefaultHeader("Authorization", $"Bearer {_tokenDataProvider.GetToken()}");
                var userToken = _tokenDataProvider.GetUserToken();
                if (string.IsNullOrWhiteSpace(userToken) == false)
                {
                    client.AddDefaultHeader("x-api-token", _tokenDataProvider.GetUserToken());
                }
            }

            // client.Authenticator = new SimpleAuthenticator("username", "foo", "password", "bar");
            // https://restsharp.dev/usage/authenticators.html#using-simpleauthenticator

            return client;
        }
    }
}
