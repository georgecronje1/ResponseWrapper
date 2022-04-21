using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Psg.Core.ApplicationInsights.DI
{
    public class ApiTelemetryInitialiser : ITelemetryInitializer
    {
        readonly TelemetryInitOptions _telemetryInitOptions;

        public ApiTelemetryInitialiser(TelemetryInitOptions telemetryInitOptions)
        {
            _telemetryInitOptions = telemetryInitOptions;
        }

        public void Initialize(ITelemetry telemetry)
{
            telemetry.Context.Cloud.RoleName = $"{_telemetryInitOptions.Environment}.{_telemetryInitOptions.AppName}";
        }
    }
}
