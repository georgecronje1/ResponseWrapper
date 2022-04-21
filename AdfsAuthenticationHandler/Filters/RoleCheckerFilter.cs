using AdfsAuthenticationHandler.Exceptions;
using AdfsAuthenticationHandler.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;


namespace Microsoft.AspNetCore.Routing
{
    public static class RouteValueDictionaryExtensions
    {
        public static string GetValue(this RouteValueDictionary keyValues, string key)
        {
            if (keyValues == null) return null;
            if (keyValues.ContainsKey(key) == false) return null;
            return keyValues[key] as string;
        }
    }
}

namespace AdfsAuthenticationHandler.Filters
{
    

    public class RoleCheckerFilter : IActionFilter
    {
        private const string _actionParameter = "action";
        private const string _controllerParameter = "controller";

        readonly IUserAccessService _userAccessService;

        public RoleCheckerFilter(IUserAccessService userAccessService)
        {
            _userAccessService = userAccessService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context?.RouteData?.Values?.GetValue(_controllerParameter);
            var actionName     = context?.RouteData?.Values?.GetValue(_actionParameter);

            PathString path = context.HttpContext.Request.Path;

            if (_userAccessService.IsPathAllowedAnonymous(controllerName, actionName,path.Value))
            {
                return;
            }

            if (_userAccessService.IsAuthenticated() == false)
            {
                context.HttpContext.Response.StatusCode = 401;
                throw new AdfsAuthException("No authenticated user found");
            }

            if (_userAccessService.HasAllRequiredTokens(controllerName, actionName, path.Value) == false)
            {
                context.HttpContext.Response.StatusCode = 401;
                throw new AdfsAuthException("No logged in user found");
            }

            if (_userAccessService.IsUserAuthorizedForPath(controllerName, actionName, path.Value) == false)
            {
                context.HttpContext.Response.StatusCode = 401;
                throw new AdfsAuthException("You are not authorized to use this API");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
