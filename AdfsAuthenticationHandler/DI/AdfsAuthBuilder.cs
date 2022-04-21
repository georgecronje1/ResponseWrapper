using Microsoft.Extensions.DependencyInjection;

namespace AdfsAuthenticationHandler.DI
{
    public class AdfsAuthBuilder
    {
        public IServiceCollection Services { get; }

        public AdfsAuthBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
