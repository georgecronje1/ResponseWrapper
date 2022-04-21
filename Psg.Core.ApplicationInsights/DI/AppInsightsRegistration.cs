using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Psg.Core.ApplicationInsights.DI
{
    public static class AppInsightsRegistration
    {
        /// <summary>
        /// Make sure to add the Instrumentation Key to AppSettings.
        /// </summary>
        /// <returns></returns>
        public static AppInsightsRegistrationBuilder AddAppInsightsTracking(this IServiceCollection services)
        {
            var builder = new AppInsightsRegistrationBuilder(services);

            builder.Services.AddApplicationInsightsTelemetry();

            return builder;
        }

        public static AppInsightsRegistrationBuilder AddTelemetryInitOptions(this AppInsightsRegistrationBuilder builder, System.Func<TelemetryInitOptions> getOptions)
        {
            var options = getOptions();

            builder.Services.AddSingleton(options);
            builder.Services.AddSingleton<ITelemetryInitializer, ApiTelemetryInitialiser>();

            return builder;
        }

        /// <summary>
        /// Remember to also add app.UseAppInsightsRequestResponseTracking()
        /// </summary>
        public static AppInsightsRegistrationBuilder AddAppInsightsTracking(this AppInsightsRegistrationBuilder builder, Action<AppInsightsTrackingOptions> getOptions)
        {
            var options = AppInsightsTrackingOptions.MakeWithDefaults();
            getOptions(options);
            builder.Services.AddSingleton(options);
            
            builder
                .AddSqlTracking(options.SqlOptions)
                .AddRequestTracking(options.RequestOptions)
                .AddResponseTracking(options.ResponseOptions);


            return builder;
        }

        
    }
}
