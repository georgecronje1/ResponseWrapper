using AdfsAuthenticationHandler.DI.Models;
using AdfsAuthenticationHandler.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResponseWrapper.Tests.AdfsAuth
{
    public class AdfsUserProviderServiceTest
    {
        Mock<IAdfsUserProviderService> _adfsUserProviderService;

        RoleCheckerConfig _roleCheckerConfig;

        UserAccessService _userAccessService;

        const string specialToken = "USER_TOKEN_NOT_REQUIRED";

        [SetUp]
        public void SetUp()
        {
            _adfsUserProviderService = new Mock<IAdfsUserProviderService>();
            _roleCheckerConfig = new RoleCheckerConfig();
            _userAccessService = new UserAccessService(_adfsUserProviderService.Object, _roleCheckerConfig);
        }

        [Test]
        public void HasAllRequiredTokens_WhenBypassIsAllowedAndHasSpecialToken_ThenReturnTrue()
        {
            _roleCheckerConfig.AddItem("systemController", "systemAction", "write").AllowUserCheckBypass();


            _adfsUserProviderService
                .Setup(a => a.DoesUserHaveAnyOfTheseRoles(It.IsAny<string[]>()))
                .Returns(true);

            var result = _userAccessService.HasAllRequiredTokens(string.Empty,string.Empty,"/api/systemController/systemAction");

            Assert.IsTrue(result);
        }

        [Test]
        public void HasAllRequiredTokens_WhenBypassIsAllowedAndHasSpecialToken_ThenReturnFalse()
        {
            _roleCheckerConfig.AddItem("systemController", "systemAction", "write").AllowUserCheckBypass();


            _adfsUserProviderService
                .Setup(a => a.DoesUserHaveAnyOfTheseRoles(It.IsAny<string[]>()))
                .Returns(false);

            var result = _userAccessService.HasAllRequiredTokens(string.Empty, string.Empty, "/api/systemController/systemAction");

            Assert.IsFalse(result);
        }
    }
}
