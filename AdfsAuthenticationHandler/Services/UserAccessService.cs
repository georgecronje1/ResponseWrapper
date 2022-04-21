using AdfsAuthenticationHandler.DI.Models;

namespace AdfsAuthenticationHandler.Services
{
    public interface IUserAccessService
    {
        bool IsAuthenticated();
        bool IsPathAllowedAnonymous(string controllerName, string actionName,string path);
        bool IsUserAuthorizedForPath(string controllerName, string actionName,string path);
        bool HasAllRequiredTokens(string controllerName, string actionName,string path);
    }

    public class UserAccessService : IUserAccessService
    {
        const string specialToken = "USER_TOKEN_NOT_REQUIRED";
        readonly IAdfsUserProviderService _adfsUserProviderService;
        readonly RoleCheckerConfig _roleCheckerConfig;

        public UserAccessService(IAdfsUserProviderService adfsUserProviderService, RoleCheckerConfig roleCheckerConfig)
        {
            _adfsUserProviderService = adfsUserProviderService;
            _roleCheckerConfig = roleCheckerConfig;
        }

        public bool IsPathAllowedAnonymous(string controllerName, string actionName, string path)
        {
            var pathConfig = _roleCheckerConfig.GetRolesForPath(controllerName, actionName,path);

            return pathConfig.AllowAnon;
        }

        public bool IsAuthenticated()
        {
            return _adfsUserProviderService.IsUserLoggedIn();
        }

        public bool IsUserAuthorizedForPath(string controllerName, string actionName,string path)
        {
            var pathConfig = _roleCheckerConfig.GetRolesForPath(controllerName, actionName, path);

            var isInRightRole = _adfsUserProviderService.DoesUserHaveAnyOfTheseRoles(pathConfig.Roles);

            return isInRightRole;
        }

        public bool HasAllRequiredTokens(string controllerName, string actionName,string path)
        {
            if (ShouldBypassUserCheckForPath(controllerName, actionName, path) && HasSpecialToken())
            {
                return true;
            }

            return _adfsUserProviderService.HasApplicationAndUserIdentity();
        }

        bool ShouldBypassUserCheckForPath(string controllerName, string actionName,string path)
        {
            var pathConfig = _roleCheckerConfig.GetRolesForPath(controllerName, actionName, path);

            return pathConfig.BypassUserTokenCheck;
        }

        bool HasSpecialToken()
        {
            var specialRole = new string[] { specialToken };
            return _adfsUserProviderService.DoesUserHaveAnyOfTheseRoles(specialRole);
        }
    }
}
