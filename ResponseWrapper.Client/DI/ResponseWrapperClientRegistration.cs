using Microsoft.Extensions.DependencyInjection;
using ResponseWrapper.Client.Services;
using System;

namespace ResponseWrapper.Client.DI
{
    public static class ResponseWrapperClientRegistration
    {
        public static void AddResponseWrapperClient(this IServiceCollection services, Action<ResponseWrapperConfig> getResponseWrapperConfig = null)
        {
            var responseWrapperConfig = new ResponseWrapperConfig();

            if(responseWrapperConfig != null)
            {
                getResponseWrapperConfig(responseWrapperConfig);
            }

            if(responseWrapperConfig.TokenProviderType == TokenProvider.DefaultHeaderProvider)
            {
                services.AddHttpContextAccessor();
                services.AddTransient<ITokenDataProvider, HeaderTokenDataProvider>();
            }

            if (responseWrapperConfig.TokenProviderType == TokenProvider.CustomHeaderProvider)
            {
                services.AddHttpContextAccessor();
            }

            if (responseWrapperConfig.TokenProviderType == TokenProvider.DefaultOauth2Provider)
            {
                services.AddSingleton(responseWrapperConfig.Oauth2TokenConfig);
                services.AddTransient<IOAuth2TokenClient, OAuth2TokenClient>();
                services.AddSingleton<ITokenDataProvider, Oauth2TokenDataProvider>();
            }

            if (responseWrapperConfig.TokenProviderType == TokenProvider.AllowAutoGenUserToken)
            {
                services.AddHttpContextAccessor();
                services.AddSingleton(responseWrapperConfig.Oauth2TokenConfig);
                services.AddTransient<IOAuth2TokenClient, OAuth2TokenClient>();
                services.AddTransient<ITokenDataProvider, AutoGenUserTokenProvider>();
            }

            services.AddSingleton(responseWrapperConfig);

            services.AddTransient<IResponseWrapperClientFactory, ResponseWrapperClientFactory>();

            services.AddTransient<IResponseWrapperClient, ResponseWrapperClient>();

            services.AddTransient<IGenericRestClient, GenericRestClient>();

            services.AddTransient<IResponseWrapperService, ResponseWrapperService>();

            services.AddTransient<IGenericRestService, GenericRestService>();
        }
    }
}
