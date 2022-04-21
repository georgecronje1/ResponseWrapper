using ResponseWrapper.Client.Services;

namespace ResponcesExamples.Services
{
    public class FakeRestClientService : IFakeRestClientService
    {
        private readonly IResponseWrapperService _responseWrapperService;
        private readonly RestClientConfig _restClientConfig;

        public FakeRestClientService(IResponseWrapperService responseWrapperService)
        {
            _responseWrapperService = responseWrapperService;
            _restClientConfig = new RestClientConfig();
        }

        public string GetMessage()
        {
            var result = _responseWrapperService.DoGet<string>("/Agency/GetAgencyByIfaCode/4163", _restClientConfig);
            return result;
        }
    }
}
