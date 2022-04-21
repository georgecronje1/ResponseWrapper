using System;

namespace Psg.Core.ApplicationInsights.DI
{
    public class RequestTrackingFilterItem
    {
        public Type Filter { get; }

        public RequestTrackingFilterItem(Type filter)
        {
            Filter = filter;
        }
    }

    public class ResponseTrackingFilterItem
    {
        public Type Filter { get; }

        public ResponseTrackingFilterItem(Type filter)
        {
            Filter = filter;
        }
    }
}
