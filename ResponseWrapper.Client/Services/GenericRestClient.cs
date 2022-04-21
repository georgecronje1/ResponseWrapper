using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResponseWrapper.Client.DI;
using ResponseWrapper.Client.Extensions;
using ResponseWrapper.Client.Models;
using ResponseWrapper.Core.Exceptions;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ResponseWrapper.Client.Services
{
    public interface IGenericRestClient
    {
        GenericResponse<T> DoGet<T>(string url, IGenericRestClientConfig config);
        Task<GenericResponse<T>> DoGetAsync<T>(string url, IGenericRestClientConfig config);
        GenericResponse<T> DoPost<T>(string url, object jsonData, IGenericRestClientConfig config);
        Task<GenericResponse<T>> DoPostAsync<T>(string url, object jsonData, IGenericRestClientConfig config);
    }

    public class GenericRestClient : IGenericRestClient
    {
        readonly IResponseWrapperClientFactory _clientFactory;
        readonly ILogger<GenericRestClient> _logger;

        public GenericRestClient(IResponseWrapperClientFactory clientFactory, ILogger<GenericRestClient> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public GenericResponse<T> DoGet<T>(string url, IGenericRestClientConfig config)
        {
            var request = new RestRequest(url, Method.GET);

            var client = _clientFactory.GetClient(config);

            _logger.LogInformation($"Calling DoGet on {url}");
            var response = client.Execute<T>(request);

            return GetResponse(response, config);
        }

        public GenericResponse<T> DoPost<T>(string url, object jsonData, IGenericRestClientConfig config)
        {

            var request = new RestRequest(url, Method.POST, DataFormat.Json);
            request.AddJsonBody(jsonData);

            var client = _clientFactory.GetClient(config);

            _logger.LogInformation($"Calling DoPost on {url}");
            var response = client.Execute<T>(request);

            return GetResponse(response, config);
        }

        public async Task<GenericResponse<T>> DoGetAsync<T>(string url, IGenericRestClientConfig config)
        {
            var request = new RestRequest(url, Method.GET);

            var client = _clientFactory.GetClient(config);

            _logger.LogInformation($"Calling DoGetAsync on {url}");
            var response = await client.ExecuteAsync<T>(request).ConfigureAwait(false);
            
            return GetResponse(response, config);
        }

        public async Task<GenericResponse<T>> DoPostAsync<T>(string url, object jsonData, IGenericRestClientConfig config)
        {

            var request = new RestRequest(url, Method.POST, DataFormat.Json);
            request.AddJsonBody(jsonData);

            var client = _clientFactory.GetClient(config);

            _logger.LogInformation($"Calling DoPostAsync on {url}");
            var response = await client.ExecuteAsync<T>(request).ConfigureAwait(false);
            

            return GetResponse(response, config);
        }

        GenericResponse<T> GetResponse<T>(IRestResponse<T> response, IGenericRestClientConfig config)
        {
            if (!response.IsSuccessful)
            {
                return HandleErrorResponse<T>(response, config);
            }
            
            return GenericResponse<T>.MakeSuccess<T>(response.Data);
        }

        GenericResponse<T> HandleErrorResponse<T>(IRestResponse response, IGenericRestClientConfig config)
        {
            if (config.ShouldThrowStandardExceptions)
            {
                ThrowStandardException<T>(response);
            }
            return GetErrorResponse<T>(response);
        }

        GenericResponse<T> GetErrorResponse<T>(IRestResponse response)
        {
            _logger.LogError($"Call Resulted with error response code: {response.StatusCode.ToString()} and content: {response.Content}");
            if (response.HasJsonContent() && string.IsNullOrWhiteSpace(response.Content) == false)
            {
                return parseContentAndReturnErrorObject<T>(response);
            }

            var errorMessage = response.ErrorMessage;
            var errorObject = GenericErrorResult.Make(response.Request.Resource, response.StatusCode.ToString());

            return GenericResponse<T>.MakeError<T>(errorObject);
        }

        static GenericResponse<T> parseContentAndReturnErrorObject<T>(IRestResponse response)
        {
            var errorMessage = $"Status code {response.StatusCode} returned when making a call to {response.Request.Resource}, with message: {response.ErrorMessage}";
            dynamic errorResult = JsonConvert.DeserializeObject<dynamic>(response.Content);

            var errorObject = GenericErrorResult.Make(response.Request.Resource, response.StatusCode.ToString(), errorResult: errorResult);

            return GenericResponse<T>.MakeError<T>(errorObject);
        }

        private void ThrowStandardException<T>(IRestResponse restResponse)
        {
            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new EntityNotFoundException(restResponse.Content);
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException(restResponse.Content);
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorisedException(restResponse.Content);
                case HttpStatusCode.Conflict:
                    throw new BusinessRuleException(restResponse.Content);
                default:
                    throw new RestException(restResponse.StatusCode, restResponse.Content);
            }
        }
    }
}
