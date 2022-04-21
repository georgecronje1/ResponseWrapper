using AdfsAuthenticationHandler.DI.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResponseWrapper.Tests.Configs
{
    public class RoleCheckerConfigTests
    {
        [TestCase("/api/test/action")]
        [TestCase("/api/test/action/5")]
        public void GetRolesForPath_WhenDefaultConfig_ThenAllPathsShouldBeBlockedAndNoRules(string path)
        {
            var config = new RoleCheckerConfig();

            var result = config.GetRolesForPath(string.Empty, string.Empty, path);

            Assert.IsFalse(result.AllowAnon);

            Assert.IsFalse(result.Roles.Any());
        }

        [TestCase("/api/test/action")]
        [TestCase("/api/test/action/5")]
        public void GetRolesForPath_WhenFallBackAllowsAnonConfig_ThenAllPathsShouldBeAllowedWithNoRules(string path)
        {
            var config = new RoleCheckerConfig();
            var fallbackConfig = ActionItemConfig.MakeForAllowAnon();
            config.SetFallbackConfig(fallbackConfig);

            var result = config.GetRolesForPath(string.Empty, string.Empty, path);

            Assert.IsTrue(result.AllowAnon);

            Assert.IsFalse(result.Roles.Any());
        }

        [TestCase("/api/test/readAction", "", false)]
        [TestCase("/api/test/readAction", "reader", true)]
        [TestCase("/api/test/otherReadAction", "reader", true)]
        [TestCase("/api/test", "reader", true)]
        public void GetRolesForPath_WhenFallBackConfigBlocksByReadRole_ThenAllPathsShouldBeBlockedExceptWhenUserHasReadRole(string path, string userRole, bool allow)
        {
            var config = new RoleCheckerConfig();
            var fallbackConfig = ActionItemConfig.MakeForRole("reader");
            config.SetFallbackConfig(fallbackConfig);

            var result = config.GetRolesForPath(string.Empty, string.Empty, path);

            Assert.IsFalse(result.AllowAnon);

            Assert.IsTrue(result.Roles.Any());

            var isRoleAllowed = result.Roles.Any(r => r == userRole);
            Assert.AreEqual(allow, isRoleAllowed);
        }



        [TestCase("/api/test/readAction", "", false)]
        [TestCase("/api/test/readAction", "reader", true)]
        [TestCase("/api/test/otherReadAction", "reader", true)]
        [TestCase("/api/test/writeAction", "reader", false)]
        [TestCase("/api/test/writeAction", "writer", true)]
        [TestCase("/api/test", "reader", true)]
        [TestCase("/api/test", "writer", false)]
        public void GetRolesForPath_WhenFallBackConfigBlocksByReadRoleAndOneActionRequiresWrite_ThenAllPathsShouldBeBlockedExceptWhenUserHasReadRole(string path, string userRole, bool allow)
        {
            var config = new RoleCheckerConfig();
            var fallbackConfig = ActionItemConfig.MakeForRole("reader");
            config.SetFallbackConfig(fallbackConfig);

            config.AddItem("test", "writeAction", "writer");

            var result = config.GetRolesForPath(string.Empty, string.Empty, path);

            Assert.IsFalse(result.AllowAnon);

            Assert.IsTrue(result.Roles.Any());

            var isRoleAllowed = result.Roles.Any(r => r == userRole);
            Assert.AreEqual(allow, isRoleAllowed);
        }

        [TestCase("/api/userController/someUserAction", "", false)]
        [TestCase("/api/userController/someUserAction", "validUserRole", true)]
        [TestCase("/api/thingyController/someAction", "validUserRole", true)]
        [TestCase("/api/adminController/someAdminAction", "validUserRole", false)]
        [TestCase("/api/adminController/otherAdminAction", "validUserRole", false)]
        [TestCase("/api/adminController/otherAdminAction", "adminRole", true)]
        [TestCase("/api/adminController", "adminRole", true)]
        [TestCase("/api/adminController", "validUserRole", false)]
        [TestCase("/api/userController", "validUserRole", true)]
        [TestCase("/api/thingyController", "validUserRole", true)]
        public void GetRolesForPath_WhenHaveUserRoleToAccessEverythingExceptForAdminController_ThenAllowUserRoleToAccessEverythingExceptAdmin(string path, string userRole, bool shouldAllow)
        {
            var config = new RoleCheckerConfig();
            var fallbackConfig = ActionItemConfig.MakeForRole("validUserRole");
            config.SetFallbackConfig(fallbackConfig);

            config.AddItem("adminController", "*", "adminRole");

            var result = config.GetRolesForPath(string.Empty, string.Empty, path);

            Assert.IsFalse(result.AllowAnon);

            Assert.IsTrue(result.Roles.Any());

            var isRoleAllowed = result.Roles.Any(r => r == userRole);
            Assert.AreEqual(shouldAllow, isRoleAllowed);
        }

        [TestCase("/api/adminController/someAdminAction", true)]
        [TestCase("/api/adminController/otherAdminAction", true)]
        [TestCase("/api/userController/otherUserAction", false)]
        [TestCase("/api/adminController", true)]
        [TestCase("/api/userController", false)]
        public void GetRolesForPath_WhenWildCardControllerConfigAdded_ThenConfigIsFoundForAnyActionOfThatController(string tryPath, bool shouldConfigExist)
        {
            var config = new RoleCheckerConfig();
            config.AddItem("adminController", "*", "adminRole");

            var result = config.GetRolesForPath(string.Empty, string.Empty, tryPath);

            var didFindConfig = result.Roles.Any(r => r == "adminRole");

            Assert.IsFalse(result.AllowAnon);

            Assert.AreEqual(shouldConfigExist, result.Roles.Any());

            Assert.AreEqual(shouldConfigExist, didFindConfig);
        }

        [Test]
        public void GetRolesForPath_WhenAddingSameActionTwoOrMoreTime_ThenThrowException()
        {
            var config = new RoleCheckerConfig();
            config.AddItem("userController", "userAction1", "validUserRole");

            Assert.Throws(typeof(System.Exception), () =>
            {
                config.AddItem("userController", "userAction1", "adminRole");
            });
        }

        [TestCase("/api/userController/getById", "reader", true)]
        [TestCase("/api/userController/getAllThings", "reader", true)]
        [TestCase("/api/userController/createNewThingy", "reader", false)]
        [TestCase("/api/userController/createNewThingy", "writer", true)]
        public void GetRolesForPath_WhenYouSetADefaultForControllerAndSetAnOverrideForAnAction_ThenShouldAllowAccordingly(string path, string userRole, bool shouldAllow)
        {
            var config = new RoleCheckerConfig();
            config.AddItem("userController", "*", "reader");
            config.AddItem("userController", "createNewThingy", "writer");

            var result = config.GetRolesForPath(string.Empty, string.Empty, path);

            Assert.IsFalse(result.AllowAnon);

            Assert.IsTrue(result.Roles.Any());

            var isRoleAllowed = result.Roles.Any(r => r == userRole);
            Assert.AreEqual(shouldAllow, isRoleAllowed);
        }

        [TestCase("/api/userController/getById/5", "reader", true)]
        [TestCase("/api/userController/getById/5/12", "reader", true)]
        [TestCase("/api/userController/getById/5/12", "writer", false)]
        [TestCase("/api/userController/getById/5/12", "lowest", false)]
        [TestCase("/api/userController/getAllThings", "lowest", true)]
        [TestCase("/api/userController/createNewThingy", "reader", false)]
        [TestCase("/api/userController/createNewThingy", "writer", true)]
        public void GetRolesForPath_WhenThereAreParameters_ThenShouldAllowAccordingly(string path, string userRole, bool shouldAllow)
        {
            var config = new RoleCheckerConfig();
            config.SetFallbackConfig("lowest");
            config.AddItem("userController", "getById", "reader");
            config.AddItem("userController", "createNewThingy", "writer");

            var result = config.GetRolesForPath(string.Empty, string.Empty, path);

            Assert.IsFalse(result.AllowAnon);

            Assert.IsTrue(result.Roles.Any());

            var isRoleAllowed = result.Roles.Any(r => r == userRole);
            Assert.AreEqual(shouldAllow, isRoleAllowed);
        }

        [TestCase("/api/userController/getById/5", "reader", true, false)]
        [TestCase("/api/userController/getById/5/12", "reader", true, false)]
        [TestCase("/api/userController/getById/5/12", "writer", false, false)]
        [TestCase("/api/userController/getById/5/12", "lowest", false, false)]
        [TestCase("/api/userController/getAllThings", "lowest", true, false)]
        [TestCase("/api/userController/createNewThingy", "reader", false, true)]
        [TestCase("/api/userController/createNewThingy", "writer", true, true)]
        public void GetRolesForPath_WhenUserCheckBypassSetOnPass_ThenAllowThatPathIfRolesMatch(string path, string userRole, bool shouldAllow, bool ShouldBypassUserCheck)
        {
            var config = new RoleCheckerConfig();
            config.SetFallbackConfig("lowest");
            config.AddItem("userController", "getById", "reader");
            config.AddItem("userController", "createNewThingy", "writer").AllowUserCheckBypass();

            var result = config.GetRolesForPath(string.Empty, string.Empty, path);

            Assert.IsFalse(result.AllowAnon);

            Assert.IsTrue(result.Roles.Any());

            var isRoleAllowed = result.Roles.Any(r => r == userRole);
            Assert.AreEqual(shouldAllow, isRoleAllowed);

            Assert.AreEqual(ShouldBypassUserCheck, result.BypassUserTokenCheck);
        }

        [TestCase("/api/recurringContribution/ABC123/create", "creator", true)]
        [TestCase("/api/recurringContribution/ABC123/create", "reader", false)]
        [TestCase("/api/recurringContribution/ABC123/getThingy", "reader", true)]
        public void GetRolesForPath_WhenParameterBeforeAction_ThenStillFindTheConfig(string path, string userRole, bool shouldAllow)
        {
            var config = new RoleCheckerConfig();
            config.SetFallbackConfig("reader");
            config.AddItem("recurringContribution", "*/create", "creator", true);

            var result = config.GetRolesForPath(string.Empty, string.Empty, path);

            var isRoleAllowed = result.Roles.Any(r => r == userRole);

            Assert.AreEqual(shouldAllow, isRoleAllowed);
        }

        [TestCase("/Authorize/doAuth", "Auth", true)]
        [TestCase("/Authorize/thingyAuth", "Auth", true)]
        [TestCase("/Authorize/authAction", "reader", false)]
        [TestCase("/Food/FooAct", "reader", false)]
        [TestCase("/Food/FooAct", "Auth", false)]
        public void GetRolesForPath_WhenApiPrefixIsEmpty_ThenApplyRolesConfig(string path, string userRole, bool shouldAllow)
        {
            var config = new RoleCheckerConfig();

            config.AddItem("Authorize", "*", "Auth");
            config.ApiPrefix = string.Empty;

            var result = config.GetRolesForPath(string.Empty, string.Empty, path);

            var isRoleAllowed = result.Roles.Any(r => r == userRole);

            Assert.AreEqual(shouldAllow, isRoleAllowed);
        }
    }
}
