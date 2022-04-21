using AdfsAuthenticationHandler.Services.Funds;
using Microsoft.Extensions.DependencyInjection;

namespace AdfsAuthenticationHandler.DI
{
    public static class FundsAuthorizationRegistration
    {
        public static AdfsAuthBuilder AddFundsAuthorization(this AdfsAuthBuilder adfsAuthBuilder)
        {
            adfsAuthBuilder.Services.AddTransient<IFundsAuthorizationService, FundsAuthorizationService>();

            return adfsAuthBuilder;
        }
    }
}
