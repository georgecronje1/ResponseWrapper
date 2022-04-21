using System.Collections.Generic;

namespace Psg.Core.ApplicationInsights.Filters
{
    public interface IRequestTrackingFilter
    {
        string ProcessBody(string path, string requestBody);
        List<RequestTrackingPropertyItem> GetExtraTrackingProperties(string path, string requestBody);
    }

    public class RequestTrackingPropertyItem
    {
        public string Name { get; }

        public string Value { get; }

        public RequestTrackingPropertyItem(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
    public interface IResponseTrackingFilter
    {
        string ProcessBody(string path, string responseBody);
        List<ResponseTrackingPropertyItem> GetExtraTrackingProperties(string path, string responseBody);
    }

    public class ResponseTrackingPropertyItem
    {
        public string Name { get; }

        public string Value { get; }

        public ResponseTrackingPropertyItem(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
