using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace ResponseWrapper.DI
{
    public static class StandardResponseRegistration
    {
        public static void AddStandardResponseRegistation(this IServiceCollection services, Action<ExceptionConfig> setupConfig = null, Action<ResponseConfig> responseConfig = null)
        {
            var config = new ExceptionConfig();
            config.SetDefaultMessage("Oops something went wrong");
            if (setupConfig != null)
            {
                setupConfig(config);
            }

            services.AddSingleton(config);

            var resultConfig = new ResponseConfig();
            if (responseConfig != null)
            {
                responseConfig(resultConfig);
            }
            services.AddSingleton(resultConfig);


            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>();
        }
    }

    public class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
    {
        public void Configure(MvcOptions options)
        {
            options.Filters.Add<Filters.ApiExceptionFilter>();
            options.Filters.Add<Filters.StandardResponseActionFilter>();
        }
    }
}
