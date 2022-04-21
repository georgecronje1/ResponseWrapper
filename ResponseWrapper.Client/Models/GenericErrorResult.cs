namespace ResponseWrapper.Client.Models
{
    public class GenericErrorResult
    {
        public string Uri { get; }

        public string StatusCode { get; }

        public object ErrorResult { get; }

        GenericErrorResult(string uri, string statusCode, object errorResult)
        {
            Uri = uri;
            StatusCode = statusCode;
            ErrorResult = errorResult;
        }

        public static GenericErrorResult Make(string uri, string statusCode, object errorResult = null)
        {
            return new GenericErrorResult(uri, statusCode, errorResult);
        }
    }
}
