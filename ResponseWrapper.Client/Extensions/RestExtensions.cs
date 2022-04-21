using RestSharp;

namespace ResponseWrapper.Client.Extensions
{
    public static class RestExtensions
    {
        public static bool HasJsonContent(this IRestResponse response)
        {
            if (string.IsNullOrWhiteSpace(response.ContentType))
            {
                return false;
            }

            if (response.ContentType.Contains("json"))
            {
                return true;
            }

            return false;
        }
    }
}
