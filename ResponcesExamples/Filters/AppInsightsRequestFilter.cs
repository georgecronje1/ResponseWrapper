using Psg.Core.ApplicationInsights.Filters;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ResponcesExamples.Filters
{
    public class AppInsightsRequestFilter : IRequestTrackingFilter
    {
        public string ProcessBody(string path, string requestBody)
        {
            if (path.Contains("api/Data/SendData"))
            {
                requestBody = Regex.Replace(requestBody, "\"DataItem1\":\"[^\"]*\"", "\"DataItem1\":\"*******\"");
            }
            return requestBody;
        }

        public List<RequestTrackingPropertyItem> GetExtraTrackingProperties(string path, string requestBody)
        {
            if (path.Contains("api/Data/SendData"))
            {
                var dataItems = Newtonsoft.Json.JsonConvert.DeserializeObject<IncomingData>(requestBody);
                return new List<RequestTrackingPropertyItem>()
                {
                    new RequestTrackingPropertyItem("DataItem2", dataItems.DataItem2)
                };
            }

            return new List<RequestTrackingPropertyItem>();
        }


        public class IncomingData
        {
            public string DataItem1 { get; set; }
            public string DataItem2 { get; set; }
        }


    }

    public class AppInsightsResponseFilter : IResponseTrackingFilter
    {
        public string ProcessBody(string path, string responseBody)
        {
            responseBody = Regex.Replace(responseBody, "\"dataItem1\":\"[^\"]*\"", "\"dataItem1\":\"*******\"");
            return responseBody;
        }

        public List<ResponseTrackingPropertyItem> GetExtraTrackingProperties(string path, string responseBody)
        {
            return new List<ResponseTrackingPropertyItem>();
        }
    }
}
