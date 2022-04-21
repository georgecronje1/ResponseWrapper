using ResponseWrapper.Client.DI;

namespace ResponcesExamples.Services
{
    public class RestClientConfig : IResponseWrapperRestClientConfig
    {
        public string BaseUrl => "https://localhost:44313/api";

        public bool UseAuthHeader => true;

        public bool ShouldThrowStandardExceptions => false;
    }
}
