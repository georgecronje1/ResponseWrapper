using ResponseWrapper.Client.DI;
using ResponseWrapper.Client.Models;
using System;
using System.Threading.Tasks;

namespace ResponseWrapper.Client.Services
{
    public interface IGenericRestService
    {
        T DoGet<T>(string url, IGenericRestClientConfig config);
        Task<T> DoGetAsync<T>(string url, IGenericRestClientConfig config);
        T DoPost<T>(string url, object jsonData, IGenericRestClientConfig config);
        Task<T> DoPostAsync<T>(string url, object jsonData, IGenericRestClientConfig config);
    }

    public class GenericRestService : IGenericRestService
    {
        readonly IGenericRestClient _genericRestClient;
        readonly ResponseWrapperConfig _responseWrapperConfig;

        public GenericRestService(IGenericRestClient genericRestClient, ResponseWrapperConfig responseWrapperConfig)
        {
            _genericRestClient = genericRestClient;
            _responseWrapperConfig = responseWrapperConfig;
        }

        public T DoGet<T>(string url, IGenericRestClientConfig config)
        {
            var result = _genericRestClient.DoGet<T>(url, config);

            return HandleResult(result);
        }

        public async Task<T> DoGetAsync<T>(string url, IGenericRestClientConfig config)
        {
            var result = await _genericRestClient.DoGetAsync<T>(url, config).ConfigureAwait(false);
            return HandleResult(result);
        }

        public T DoPost<T>(string url, object jsonData, IGenericRestClientConfig config)
        {
            var result = _genericRestClient.DoPost<T>(url, jsonData, config);

            return HandleResult(result);
        }

        public async Task<T> DoPostAsync<T>(string url, object jsonData, IGenericRestClientConfig config)
        {
            var result = await _genericRestClient.DoPostAsync<T>(url, jsonData, config).ConfigureAwait(false);

            return HandleResult(result);
        }

        T HandleResult<T>(GenericResponse<T> result)
        {
            if (result.IsSuccess)
            {
                return result.Data;
            }

            var ex = _responseWrapperConfig.GenericRestErrorHandler(result.ErrorResult);
            throw ex;
        }
    }
}
