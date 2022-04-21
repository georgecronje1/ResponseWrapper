using Newtonsoft.Json;
using ResponseWrapper.Client.DI;
using ResponseWrapper.Client.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ResponseWrapper.Client.Services
{
    public interface IResponseWrapperService
    {
        T DoGet<T>(string url, IResponseWrapperRestClientConfig config);
        Task<T> DoGetAsync<T>(string url, IResponseWrapperRestClientConfig config);
        T DoPost<T>(string url, object jsonData, IResponseWrapperRestClientConfig config);
        Task<T> DoPostAsync<T>(string url, object jsonData, IResponseWrapperRestClientConfig config);
    }

    public class ResponseWrapperService : IResponseWrapperService
    {
        readonly IResponseWrapperClient _contractsClient;
        readonly ResponseWrapperConfig _responseWrapperConfig;

        public ResponseWrapperService(IResponseWrapperClient contractsClient, ResponseWrapperConfig responseWrapperConfig)
        {
            _contractsClient = contractsClient;
            _responseWrapperConfig = responseWrapperConfig;
        }

        public async Task<T> DoGetAsync<T>(string url, IResponseWrapperRestClientConfig config)
        {
            var result = await _contractsClient.DoGetAsync<T>(url, config).ConfigureAwait(false);
            return HandleResult(result);
        }

        public T DoGet<T>(string url, IResponseWrapperRestClientConfig config)
        {
            var result = _contractsClient.DoGet<T>(url, config);

            return HandleResult(result);
        }

        public async Task<T> DoPostAsync<T>(string url, object jsonData, IResponseWrapperRestClientConfig config)
        {
            var result = await _contractsClient.DoPostAsync<T>(url, jsonData, config).ConfigureAwait(false);

            return HandleResult(result);
        }

        public T DoPost<T>(string url, object jsonData, IResponseWrapperRestClientConfig config)
        {
            var result = _contractsClient.DoPost<T>(url, jsonData, config);

            return HandleResult(result);
        }

        T HandleResult<T>(StandardResponse<T> result)
        {
            if (result.ResponseType != ResponseType.Success)
            {
                HandleNonSuccess(result);
            }

            return result.Result;
        }

        void HandleNonSuccess<T>(StandardResponse<T> result)
        {
            var response = new StandardResponse<object>
            {
                ResponseType = result.ResponseType,
                Message = result.Message,
                ValidationErrors = result.ValidationErrors,
                Error = result.Error,
                Result = result.Result
            };
            var ex = _responseWrapperConfig.ResponseWrapperRestErrorHandler(response);

            throw ex;
        }
    }
}
