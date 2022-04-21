using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using ResponseWrapper.Client.DI;
using ResponseWrapper.Client.Services;
using System.Linq;

namespace ResponseWrapper.Tests.ResponseWrapperClient
{
    [TestFixture]
    public class ClientFactoryTests
    {
        private const string mockAccessToken = "mock_access_token";
        private const string mockUserAccessToken = "mock_user_access_token";

        delegate bool HttpContextAccessorReturns(string header, out StringValues headerValue);
        delegate void HttpContextAccessorCallback(string header, out StringValues headerValue);

        [Test]
        public void UseDefaultHeaderTest()
        {
            // Setup mock HttpContextAccessor
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor
                .Setup(m => m.HttpContext.Request.Headers.TryGetValue("Authorization", out It.Ref<StringValues>.IsAny))
                .Callback(new HttpContextAccessorCallback((string header, out StringValues headerValue) =>
                {
                    headerValue = mockAccessToken;
                }))
                .Returns(new HttpContextAccessorReturns((string header, out StringValues headerValue) => true));

            mockHttpContextAccessor
                .Setup(m => m.HttpContext.Request.Headers.TryGetValue("x-api-token", out It.Ref<StringValues>.IsAny))
                .Callback(new HttpContextAccessorCallback((string header, out StringValues headerValue) =>
                {
                    headerValue = mockUserAccessToken;
                }))
                .Returns(new HttpContextAccessorReturns((string header, out StringValues headerValue) => true));

            // Setup mock HeaderTokenDataProvider
            HeaderTokenDataProvider headerTokenDataProvider = new HeaderTokenDataProvider(mockHttpContextAccessor.Object);

            // Setup mock ResponseWrapperRestClientConfig
            var mockeRestClientConfig = new Mock<IResponseWrapperRestClientConfig>();
            mockeRestClientConfig.SetupGet(m => m.BaseUrl).Returns("https://mock_url/");
            mockeRestClientConfig.SetupGet(m => m.UseAuthHeader).Returns(true);

            // Setup mock ResponseWrapperRestClientConfig
            var mockConfig = new Mock<ResponseWrapperConfig>();
            mockConfig.Object.UseDefaultHeaderProvider();
            
            // Setup mock client factory
            ResponseWrapperClientFactory responseWrapperClientFactory = new ResponseWrapperClientFactory(headerTokenDataProvider, mockConfig.Object);

            // Get client
            var restClient = responseWrapperClientFactory.GetClient(mockeRestClientConfig.Object);

            // Read header
            string createdHeader = restClient.DefaultParameters.Where(d => d.Name == "Authorization").FirstOrDefault()?.Value.ToString();
            string userTokenHeader = restClient.DefaultParameters.Where(d => d.Name == "x-api-token").FirstOrDefault()?.Value.ToString();

            Assert.IsNotNull(createdHeader);
            Assert.IsNotNull(userTokenHeader);

            Assert.AreEqual($"Bearer {mockAccessToken}", createdHeader);
            Assert.AreEqual(mockUserAccessToken, userTokenHeader);
        }

        [Test]
        public void UseCustomHeaderProviderTest()
        {
            CustomHeaderProvider headerTokenDataProvider = new CustomHeaderProvider();

            // Setup mock ResponseWrapperRestClientConfig
            var mockeRestClientConfig = new Mock<IResponseWrapperRestClientConfig>();
            mockeRestClientConfig.SetupGet(m => m.BaseUrl).Returns("https://mock_url/");
            mockeRestClientConfig.SetupGet(m => m.UseAuthHeader).Returns(true);

            // Setup mock ResponseWrapperRestClientConfig
            var mockConfig = new Mock<ResponseWrapperConfig>();
            mockConfig.Object.UseCustomHeaderProvider();

            // Setup mock client factory
            ResponseWrapperClientFactory responseWrapperClientFactory = new ResponseWrapperClientFactory(headerTokenDataProvider, mockConfig.Object);

            // Get client
            var restClient = responseWrapperClientFactory.GetClient(mockeRestClientConfig.Object);

            // Read header
            string createdHeader = restClient.DefaultParameters.Where(d => d.Name == "Authorization").FirstOrDefault()?.Value.ToString();
            string userTokenHeader = restClient.DefaultParameters.Where(d => d.Name == "x-api-token").FirstOrDefault()?.Value.ToString();

            Assert.IsNotNull(createdHeader);
            Assert.IsNotNull(userTokenHeader);

            Assert.AreEqual("Bearer CustomAppToken", createdHeader);
            Assert.AreEqual("CustomUserToken", userTokenHeader);
        }

        [Test]
        public void UseOAuthTokenProviderTest()
        {

            Mock<ITokenDataProvider> oauthTokenProvider = new Mock<ITokenDataProvider>();

            oauthTokenProvider
                .Setup(x => x.GetToken())
                .Returns(mockAccessToken);

            oauthTokenProvider
                .Setup(x => x.GetUserToken())
                .Returns(mockUserAccessToken);

            var mockConfig = new ResponseWrapperConfig();
            mockConfig.UseDefaultOAuth2TokenProvider(It.IsAny<Oauth2TokenConfig>());

            // Setup mock ResponseWrapperRestClientConfig
            var mockeRestClientConfig = new Mock<IResponseWrapperRestClientConfig>();
            mockeRestClientConfig.SetupGet(m => m.BaseUrl).Returns("https://mock_url/");
            mockeRestClientConfig.SetupGet(m => m.UseAuthHeader).Returns(true);


            ResponseWrapperClientFactory responseWrapperClientFactory = new ResponseWrapperClientFactory(oauthTokenProvider.Object, mockConfig);

            // Get client
            var restClient = responseWrapperClientFactory.GetClient(mockeRestClientConfig.Object);

            // Read header
            string createdHeader = restClient.DefaultParameters.Where(d => d.Name == "Authorization").FirstOrDefault()?.Value.ToString();
            string userTokenHeader = restClient.DefaultParameters.Where(d => d.Name == "x-api-token").FirstOrDefault()?.Value.ToString();

            Assert.IsNotNull(createdHeader);
            Assert.IsNotNull(userTokenHeader);

            Assert.AreEqual($"Bearer {mockAccessToken}", createdHeader);
            Assert.AreEqual(mockUserAccessToken, userTokenHeader);
        }

        [Test]
        public void TestAutoGenUserToken()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor
                .Setup(m => m.HttpContext.Request.Headers.TryGetValue("Authorization", out It.Ref<StringValues>.IsAny))
                .Callback(new HttpContextAccessorCallback((string header, out StringValues headerValue) =>
                {
                    headerValue = mockAccessToken;
                }))
                .Returns(new HttpContextAccessorReturns((string header, out StringValues headerValue) => true));

            var oauth2Clientmock = new Mock<IOAuth2TokenClient>();
            oauth2Clientmock
                .Setup(c => c.GetNewUserToken())
                .Returns(new TokenResponse { access_token = mockUserAccessToken });

            // Setup mock HeaderTokenDataProvider
            AutoGenUserTokenProvider autoGenUserTokenProvider = new AutoGenUserTokenProvider(oauth2Clientmock.Object, mockHttpContextAccessor.Object);

            // Setup mock ResponseWrapperRestClientConfig
            var mockeRestClientConfig = new Mock<IResponseWrapperRestClientConfig>();
            mockeRestClientConfig.SetupGet(m => m.BaseUrl).Returns("https://mock_url/");
            mockeRestClientConfig.SetupGet(m => m.UseAuthHeader).Returns(true);

            var mockConfig = new Mock<ResponseWrapperConfig>();
            mockConfig.Object.UseAutoGenUserTokenProvider(AuthGenUserSettings.Make(It.IsAny<string>(), It.IsAny<Oauth2TokenConfig.ApplicationTokenSettings>()));

            ResponseWrapperClientFactory responseWrapperClientFactory = new ResponseWrapperClientFactory(autoGenUserTokenProvider, mockConfig.Object);

            // Get client
            var restClient = responseWrapperClientFactory.GetClient(mockeRestClientConfig.Object);

            // Read header
            string createdHeader = restClient.DefaultParameters.Where(d => d.Name == "Authorization").FirstOrDefault()?.Value.ToString();
            string userTokenHeader = restClient.DefaultParameters.Where(d => d.Name == "x-api-token").FirstOrDefault()?.Value.ToString();

            Assert.IsNotNull(createdHeader);
            Assert.IsNotNull(userTokenHeader);

            Assert.AreEqual($"Bearer {mockAccessToken}", createdHeader);
            Assert.AreEqual(mockUserAccessToken, userTokenHeader);
        }

        [Test]
        public void UseNoAuthTest()
        {
            // Setup mock ResponseWrapperRestClientConfig
            var mockConfig = new Mock<ResponseWrapperConfig>();

            // Setup mock ResponseWrapperRestClientConfig
            var mockeRestClientConfig = new Mock<IResponseWrapperRestClientConfig>();
            mockeRestClientConfig.SetupGet(m => m.BaseUrl).Returns("https://mock_url/");
            mockeRestClientConfig.SetupGet(m => m.UseAuthHeader).Returns(true);

            // Setup mock client factory
            ResponseWrapperClientFactory responseWrapperClientFactory = new ResponseWrapperClientFactory(null, mockConfig.Object);

            // Get client
            var restClient = responseWrapperClientFactory.GetClient(mockeRestClientConfig.Object);

            // Read header
            var headerSearch = restClient.DefaultParameters.Where(d => d.Name == "Authorization");

            Assert.AreEqual(false, headerSearch.Any());
        }
    }

    public class CustomHeaderProvider : ITokenDataProvider
    {
        public string GetToken()
        {
            return "CustomAppToken";
        }

        public string GetUserToken()
        {
            return "CustomUserToken";
        }
    }
}
