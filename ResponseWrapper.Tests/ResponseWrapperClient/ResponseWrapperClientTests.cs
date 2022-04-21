using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ResponseWrapper.Client.DI;
using ResponseWrapper.Client.Models;
using ResponseWrapper.Client.Services;
using ResponseWrapper.Core.Exceptions;
using RestSharp;
using System;
using System.Net;

namespace ResponseWrapper.Tests.ResponseWrapperClient
{
    [TestFixture]
    public class ResponseWrapperClientTests
    {
        [Test]
        public void ErorrResultWithEmptyContentTest()
        {
            var restResponse = new RestResponse()
            {
                Content     = string.Empty,
                StatusCode  = HttpStatusCode.Unauthorized,
                ContentType = "text",
                Request     = new RestRequest() { Resource = "fakeResource"}
            };

            // IResponseWrapperClientFactory not needed for the method we are testing 
            var responseWrapperClient = new Client.Services.ResponseWrapperClient(default(IResponseWrapperClientFactory), new NullLogger<Client.Services.ResponseWrapperClient>());

            var stdResponse = responseWrapperClient.GetErrorResponse<dynamic>(restResponse);

            var errorObject = (GenericErrorResult)stdResponse.Error.ErrorResult;

            Assert.AreEqual("fakeResource", errorObject.Uri);
            Assert.AreEqual(HttpStatusCode.Unauthorized.ToString(), errorObject.StatusCode);
        }

        [Test]
        public void ErorrResultWithContentObjectTest()
        {
            
            var restResponse = new RestResponse()
            {
                Content     = "{ \"field1\": 1, \"field2\": \"Two\"}",
                StatusCode  = HttpStatusCode.Unauthorized,
                ContentType = "application\\json",
                Request     = new RestRequest() { Resource = "fakeResource" }
            };

            // IResponseWrapperClientFactory not needed for the method we are testing 
            var responseWrapperClient = new Client.Services.ResponseWrapperClient(default(IResponseWrapperClientFactory), new NullLogger<Client.Services.ResponseWrapperClient>());

            var stdResponse = responseWrapperClient.GetErrorResponse<dynamic>(restResponse);

            var errorObject = stdResponse.Error.ErrorResult as JObject;

            Assert.AreEqual(1, ((JValue)errorObject["field1"]).Value);
            Assert.AreEqual("Two", ((JValue)errorObject["field2"]).Value);
        }

        [Test]
        public void ThrowsEntityNotFoundException()
        {
            const string contentMessage = "Requested resource could not be found";
            var response = new StandardResponse<string>(
                    ResponseType.ExceptionError,
                    contentMessage,
                    new ValidationError[0],
                    null,
                    new Error() { ErrorMessage = contentMessage, ErrorResult = null }
                );
            string content = JsonConvert.SerializeObject(response); 
            IRestResponse<StandardResponse<string>> apiResponse = new RestResponse<StandardResponse<string>> 
            { 
                StatusCode = HttpStatusCode.NotFound, 
                Content = content, 
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = response
            };
            var mockResponseWrapperClient = BuildMockResponseWrapperClient(apiResponse);

            var ex = Assert.Throws<EntityNotFoundException>(() => mockResponseWrapperClient.DoGet<string>("", new MockResponseWrapperSettings()));
        }

        [Test]
        public void ThrowsBadRequestException()
        {
            const string contentMessage = "You're missing something...";
            var response = new StandardResponse<string>(
                    ResponseType.ExceptionError,
                    contentMessage,
                    new ValidationError[0],
                    null,
                    new Error() { ErrorMessage = contentMessage, ErrorResult = null }
                );
            string content = JsonConvert.SerializeObject(response); 
            IRestResponse<StandardResponse<string>> apiResponse = new RestResponse<StandardResponse<string>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = content,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = response
            };
            var mockResponseWrapperClient = BuildMockResponseWrapperClient(apiResponse);

            var ex = Assert.Throws<BadRequestException>(() => mockResponseWrapperClient.DoGet<string>("", new MockResponseWrapperSettings()));
            Assert.AreEqual(contentMessage, ex.Message);
        }

        [Test]
        public void ThrowsBusinessRuleException()
        {
            const string contentMessage = "We don't do that here...";
            var response = new StandardResponse<string>(
                    ResponseType.ExceptionError,
                    contentMessage,
                    new ValidationError[0],
                    null,
                    new Error() { ErrorMessage = contentMessage, ErrorResult = null }
                );
            string content = JsonConvert.SerializeObject(response);

            IRestResponse<StandardResponse<string>> apiResponse = new RestResponse<StandardResponse<string>>
            {
                StatusCode = HttpStatusCode.Conflict,
                Content = content,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = response
            };
            var mockResponseWrapperClient = BuildMockResponseWrapperClient(apiResponse);

            var ex = Assert.Throws<BusinessRuleException>(() => mockResponseWrapperClient.DoGet<string>("", new MockResponseWrapperSettings()));
            Assert.AreEqual(contentMessage, ex.Message);
        }

        [Test]
        public void ThrowsUnauthorisedException()
        {
            const string contentMessage = "You shall not pass";
            var response = new StandardResponse<string>(
                    ResponseType.ExceptionError,
                    contentMessage,
                    new ValidationError[0],
                    null,
                    new Error() { ErrorMessage = contentMessage, ErrorResult = null }
                );
            string content = JsonConvert.SerializeObject(response);

            IRestResponse<StandardResponse<string>> apiResponse = new RestResponse<StandardResponse<string>>
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = content,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = response
            };
            var mockResponseWrapperClient = BuildMockResponseWrapperClient(apiResponse);

            var ex = Assert.Throws<UnauthorisedException>(() => mockResponseWrapperClient.DoGet<string>("", new MockResponseWrapperSettings()));
            Assert.AreEqual(contentMessage, ex.Message);
        }

        [TestCase("A bad gateway exception", HttpStatusCode.BadGateway)]
        [TestCase("An internal server error exception", HttpStatusCode.InternalServerError)]
        [TestCase("An redirect exception", HttpStatusCode.Redirect)]
        public void ThrowsRestException(string contentMessage, HttpStatusCode httpStatusCode)
        {
            var response = new StandardResponse<string>(
                    ResponseType.ExceptionError,
                    contentMessage,
                    new ValidationError[0],
                    null,
                    new Error() { ErrorMessage = contentMessage, ErrorResult = null }
                );
            string content = JsonConvert.SerializeObject(response);

            IRestResponse<StandardResponse<string>> apiResponse = new RestResponse<StandardResponse<string>>
            {
                StatusCode = httpStatusCode,
                Content = content,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = response
            };
            var mockResponseWrapperClient = BuildMockResponseWrapperClient(apiResponse);

            var ex = Assert.Throws<RestException>(() => mockResponseWrapperClient.DoGet<string>("", new MockResponseWrapperSettings()));
            Assert.AreEqual(httpStatusCode, ex.HttpStatusCode);
            Assert.AreEqual(contentMessage, ex.Message);
        }

        [TestCase("A successful GET with 200 response", HttpStatusCode.OK)]
        [TestCase("A successful GET with 204 response", HttpStatusCode.NoContent)]
        [TestCase("A successful GET with 201 response", HttpStatusCode.Created)]
        [TestCase("A successful GET with 202 response", HttpStatusCode.Accepted)]
        public void ProcessesSuccessfulRequest(string contentMessage, HttpStatusCode httpStatusCode)
        {
            var response = new StandardResponse<string>(
                    ResponseType.Success,
                    contentMessage,
                    new ValidationError[0],
                    contentMessage,
                    null
                );
            string content = JsonConvert.SerializeObject(response);

            IRestResponse<StandardResponse<string>> apiResponse = new RestResponse<StandardResponse<string>>
            {
                StatusCode = httpStatusCode,
                Content = content,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = response
            };
            var mockResponseWrapperClient = BuildMockResponseWrapperClient(apiResponse);

            var result = mockResponseWrapperClient.DoGet<string>("", new MockResponseWrapperSettings());
            Assert.AreEqual(contentMessage, result.Result);
        }

        private static Client.Services.ResponseWrapperClient BuildMockResponseWrapperClient(IRestResponse<StandardResponse<string>> apiResponse)
        {
            var mockRestClient = new Mock<RestClient>();
            mockRestClient
                .SetupSet(x => x.BaseUrl = new Uri("https://wealth-dev.psg.co.za/secure/"));
            mockRestClient
                .Setup(x => x.Execute<StandardResponse<string>>(It.IsAny<IRestRequest>()))
                .Returns(apiResponse);

            var mockClientFactory = new Mock<IResponseWrapperClientFactory>();
            mockClientFactory
                .Setup(m => m.GetClient(It.IsAny<IResponseWrapperRestClientConfig>()))
                .Returns(mockRestClient.Object);

            var mockResponseWrapperClient = new Client.Services.ResponseWrapperClient(mockClientFactory.Object, new NullLogger<Client.Services.ResponseWrapperClient>());
            return mockResponseWrapperClient;
        }
    }

    public class MockResponseWrapperSettings : IResponseWrapperRestClientConfig
    {
        public string BaseUrl => "FakeUrl";

        public bool UseAuthHeader => true;

        public bool ShouldThrowStandardExceptions => true;
    }
}
