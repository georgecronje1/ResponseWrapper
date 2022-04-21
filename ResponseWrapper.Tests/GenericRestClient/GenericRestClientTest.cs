using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using ResponseWrapper.Client.DI;
using ResponseWrapper.Client.Services;
using ResponseWrapper.Core.Exceptions;
using RestSharp;
using System;
using System.Net;

namespace ResponseWrapper.Tests.GenericRestClient
{
    [TestFixture]
    public class GenericRestClientTest
    {
        [Test]
        public void ThrowsEntityNotFoundException()
        {
            const string contentMessage = "Requested resource could not be found";
            string content = JsonConvert.SerializeObject(contentMessage);
            IRestResponse<string> apiResponse = new RestResponse<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = content,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = contentMessage
            };
            var mockGenericRestClient = BuildMockGenericRestClient(apiResponse);

            var ex = Assert.Throws<EntityNotFoundException>(() => mockGenericRestClient.DoGet<string>("", new MockGenericRestClientSettings()));
        }

        [Test]
        public void ThrowsBadRequestException()
        {
            const string contentMessage = "You're missing something...";
            IRestResponse<string> apiResponse = new RestResponse<string>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = contentMessage,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = contentMessage
            };
            var mockGenericRestClient = BuildMockGenericRestClient(apiResponse);

            var ex = Assert.Throws<BadRequestException>(() => mockGenericRestClient.DoGet<string>("", new MockGenericRestClientSettings()));
            Assert.AreEqual(contentMessage, ex.Message);
        }

        [Test]
        public void ThrowsBusinessRuleException()
        {
            const string contentMessage = "We don't do that here...";
            
            IRestResponse<string> apiResponse = new RestResponse<string>
            {
                StatusCode = HttpStatusCode.Conflict,
                Content = contentMessage,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = contentMessage
            };
            var mockGenericRestClient = BuildMockGenericRestClient(apiResponse);

            var ex = Assert.Throws<BusinessRuleException>(() => mockGenericRestClient.DoGet<string>("", new MockGenericRestClientSettings()));
            Assert.AreEqual(contentMessage, ex.Message);
        }

        [Test]
        public void ThrowsUnauthorisedException()
        {
            const string contentMessage = "You shall not pass";
            
            IRestResponse<string> apiResponse = new RestResponse<string>
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = contentMessage,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = contentMessage
            };
            var mockGenericRestClient = BuildMockGenericRestClient(apiResponse);

            var ex = Assert.Throws<UnauthorisedException>(() => mockGenericRestClient.DoGet<string>("", new MockGenericRestClientSettings()));
            Assert.AreEqual(contentMessage, ex.Message);
        }

        [TestCase("A bad gateway exception", HttpStatusCode.BadGateway)]
        [TestCase("An internal server error exception", HttpStatusCode.InternalServerError)]
        [TestCase("An redirect exception", HttpStatusCode.Redirect)]
        public void ThrowsRestException(string contentMessage, HttpStatusCode httpStatusCode)
        {
            IRestResponse<string> apiResponse = new RestResponse<string>
            {
                StatusCode = httpStatusCode,
                Content = contentMessage,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = contentMessage
            };
            var mockGenericRestClient = BuildMockGenericRestClient(apiResponse);

            var ex = Assert.Throws<RestException>(() => mockGenericRestClient.DoGet<string>("", new MockGenericRestClientSettings()));
            Assert.AreEqual(httpStatusCode, ex.HttpStatusCode);
            Assert.AreEqual(contentMessage, ex.Message);
        }

        [TestCase("A successful GET with 200 response", HttpStatusCode.OK)]
        [TestCase("A successful GET with 204 response", HttpStatusCode.NoContent)]
        [TestCase("A successful GET with 201 response", HttpStatusCode.Created)]
        [TestCase("A successful GET with 202 response", HttpStatusCode.Accepted)]
        public void ProcessesSuccessfulRequest(string contentMessage, HttpStatusCode httpStatusCode)
        {
            IRestResponse<string> apiResponse = new RestResponse<string>
            {
                StatusCode = httpStatusCode,
                Content = contentMessage,
                ContentType = "application/json",
                ResponseStatus = ResponseStatus.Completed,
                Data = contentMessage
            };
            Client.Services.GenericRestClient mockGenericRestClient = BuildMockGenericRestClient(apiResponse);

            var result = mockGenericRestClient.DoGet<string>("", new MockGenericRestClientSettings());
            Assert.AreEqual(contentMessage, result.Data);
        }

        private static Client.Services.GenericRestClient BuildMockGenericRestClient(IRestResponse<string> apiResponse)
        {
            var mockRestClient = new Mock<RestClient>();
            mockRestClient
                .SetupSet(x => x.BaseUrl = new Uri("https://wealth-dev.psg.co.za/secure/"));
            mockRestClient
                .Setup(x => x.Execute<string>(It.IsAny<IRestRequest>()))
                .Returns(apiResponse);

            var mockClientFactory = new Mock<IResponseWrapperClientFactory>();
            mockClientFactory
                .Setup(m => m.GetClient(It.IsAny<IGenericRestClientConfig>()))
                .Returns(mockRestClient.Object);

            var mockGenericRestClient = new Client.Services.GenericRestClient(mockClientFactory.Object, new NullLogger<Client.Services.GenericRestClient>());
            return mockGenericRestClient;
        }
    }

    public class MockGenericRestClientSettings : IGenericRestClientConfig
    {
        public string BaseUrl => "FakeUrl";

        public bool UseAuthHeader => true;

        public bool ShouldThrowStandardExceptions => true;
    }
}
