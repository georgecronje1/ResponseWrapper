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
    public interface IResponseWrapperClient
    {
        StandardResponse<T> DoGet<T>(string url, IResponseWrapperRestClientConfig config);
        Task<StandardResponse<T>> DoGetAsync<T>(string url, IResponseWrapperRestClientConfig config);
        StandardResponse<T> DoPost<T>(string url, object jsonData, IResponseWrapperRestClientConfig config);
        Task<StandardResponse<T>> DoPostAsync<T>(string url, object jsonData, IResponseWrapperRestClientConfig config);
        StandardResponse<T> GetErrorResponse<T>(IRestResponse response);
    }

    public class ResponseWrapperClient : IResponseWrapperClient
    {
        readonly IResponseWrapperClientFactory _clientFactory;
        readonly ILogger<ResponseWrapperClient> _logger;

        public ResponseWrapperClient(IResponseWrapperClientFactory clientFactory, ILogger<ResponseWrapperClient> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<StandardResponse<T>> DoGetAsync<T>(string url, IResponseWrapperRestClientConfig config)
        {
            var request = new RestRequest(url, Method.GET);

            var client = _clientFactory.GetClient(config);

            _logger.LogInformation($"Calling DoGetAsync on {url}");
            var response = await client.ExecuteAsync<StandardResponse<T>>(request).ConfigureAwait(false);

            if(!response.IsSuccessful)
            {
                return HandleErrorResponse<T>(response, config);
            }

            return response.Data;
        }

        public StandardResponse<T> DoGet<T>(string url, IResponseWrapperRestClientConfig config)
        {
            var request = new RestRequest(url, Method.GET);

            var client = _clientFactory.GetClient(config);

            _logger.LogInformation($"Calling DoGet on {url}");
            var response = client.Execute<StandardResponse<T>>(request);

            if (!response.IsSuccessful)
            {
                return HandleErrorResponse<T>(response, config);
            }
            
            return response.Data;
        }

        public async Task<StandardResponse<T>> DoPostAsync<T>(string url, object jsonData, IResponseWrapperRestClientConfig config)
        {
            var request = new RestRequest(url, Method.POST, DataFormat.Json);
            request.AddJsonBody(jsonData);

            var client = _clientFactory.GetClient(config);

            _logger.LogInformation($"Calling DoPostAsync on {url}");
            var response = await client.ExecuteAsync<StandardResponse<T>>(request).ConfigureAwait(false);

            if (!response.IsSuccessful)
            {
                return HandleErrorResponse<T>(response, config);
            }

            return response.Data;
        }

        public StandardResponse<T> DoPost<T>(string url, object jsonData, IResponseWrapperRestClientConfig config)
        {

            var request = new RestRequest(url, Method.POST, DataFormat.Json);
            request.AddJsonBody(jsonData);

            var client = _clientFactory.GetClient(config);

            _logger.LogInformation($"Calling DoPost on {url}");
            var response = client.Execute<StandardResponse<T>>(request);

            if (!response.IsSuccessful)
            {
                return HandleErrorResponse<T>(response, config);
            }

            return response.Data;
        }

        public StandardResponse<T> HandleErrorResponse<T>(IRestResponse response, IResponseWrapperRestClientConfig config)
        {
            if (config.ShouldThrowStandardExceptions)
            {
                ThrowStandardException<T>(response);
            }
            return GetErrorResponse<T>(response);
        }

        public StandardResponse<T> GetErrorResponse<T>(IRestResponse response)
        {
            _logger.LogError($"Call Resulted with error response code: {response.StatusCode.ToString()} and content: {response.Content}");
            if (response.HasJsonContent() && string.IsNullOrWhiteSpace(response.Content) == false)
            {
                return parseContentAndReturnErrorObject<T>(response);
            }

            var errorMessage = response.ErrorMessage;
            var errorObject = GenericErrorResult.Make(response.Request.Resource, response.StatusCode.ToString());

            return StandardResponse<T>.MakeForError<T>(errorMessage, errorObject);
        }

        private static StandardResponse<T> parseContentAndReturnErrorObject<T>(IRestResponse response)
        {
            var errorMessage = $"Status code {response.StatusCode} returned when making a call to {response.Request.Resource}, with message: {response.ErrorMessage}";
            dynamic errorObject = JsonConvert.DeserializeObject<dynamic>(response.Content);

            return StandardResponse<T>.MakeForError<T>(errorMessage, errorObject);
        }

        private void ThrowStandardException<T>(IRestResponse restResponse)
        {
            StandardResponse<T> standardResponse = null;
            // Try and deserialise the Standard Response
            if (restResponse.HasJsonContent() && string.IsNullOrWhiteSpace(restResponse.Content) == false)
            {
                try
                {
                    standardResponse = JsonConvert.DeserializeObject<StandardResponse<T>>(restResponse.Content);
                }
                catch (Exception)
                {
                    standardResponse = StandardResponse<T>.MakeForError<T>("An error has occurred.", restResponse.Content);
                }
            }
            if (standardResponse == null)
            {
                standardResponse = StandardResponse<T>.MakeForError<T>("An error has occurred.", restResponse.Content);
            }

            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new EntityNotFoundException(standardResponse.Message);
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException(standardResponse.Message);
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorisedException(standardResponse.Message);
                case HttpStatusCode.Conflict:
                    throw new BusinessRuleException(standardResponse.Message);
                default:
                    throw new RestException(restResponse.StatusCode, standardResponse.Message);
            }
        }
    }
}
