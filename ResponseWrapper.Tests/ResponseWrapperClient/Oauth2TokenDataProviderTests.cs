using Moq;
using NUnit.Framework;
using ResponseWrapper.Client.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResponseWrapper.Tests.ResponseWrapperClient
{
    public class Oauth2TokenDataProviderTests
    {
        Mock<IOAuth2TokenClient> _tokenClient;

        const string mockToken = "mock_token";

        [SetUp]
        public void SetUp()
        {
            _tokenClient = new Mock<IOAuth2TokenClient>();
        }

        [TestCase(30, 2)]
        [TestCase(60, 2)]
        [TestCase(61, 1)]
        [TestCase(90, 1)]
        [TestCase(120, 1)]
        public void Given_WhenTokenExpiresInLessThanOneMinuteToken_ThenMustBeReFreshed(int expiresIn, int timesCalled)
        {
            _tokenClient.Setup(c => c.GetNewToken())
                .Returns(new TokenResponse { access_token = mockToken, expires_in = expiresIn });

            var tokenProvider = new Oauth2TokenDataProvider(_tokenClient.Object);

            var result1 = tokenProvider.GetToken();

            var result2 = tokenProvider.GetToken();

            _tokenClient.Verify(c => c.GetNewToken(), times: Times.Exactly(timesCalled));

            Assert.AreEqual(mockToken, result1);
            Assert.AreEqual(mockToken, result2);
        }
    }
}
