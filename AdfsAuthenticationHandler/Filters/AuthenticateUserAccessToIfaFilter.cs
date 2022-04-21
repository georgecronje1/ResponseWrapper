using AdfsAuthenticationHandler.Exceptions;
using AdfsAuthenticationHandler.Services.Funds;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace AdfsAuthenticationHandler.Filters
{
    public class AuthenticateUserAccessToIfaFilter : ActionFilterAttribute
    {
        public string IfaCodeParameterName { get; set; }

        public AuthenticateUserAccessToIfaFilter()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrWhiteSpace(this.IfaCodeParameterName))
            {
                throw new System.Exception($"IfaCodeParameterName number cannot be empty");
            }

            var ifaCode = context.ActionArguments.SingleOrDefault(a => a.Key == this.IfaCodeParameterName);

            if (string.IsNullOrWhiteSpace(ifaCode.Value?.ToString()))
            {
                throw new System.Exception($"IFA Code could not be found for parameter name: {this.IfaCodeParameterName}");
            }

            var isEnumberable = ifaCode.Value as IEnumerable<string> != null;
            var ifaResult = isEnumberable ? (List<string>)ifaCode.Value : new List<string>() { ifaCode.Value.ToString() };

            var fundsAuthorizationServiceResult = context.HttpContext.RequestServices.GetService(typeof(IFundsAuthorizationService));
            if (fundsAuthorizationServiceResult == null)
            {
                throw new System.Exception($"Fund authorization service not instantiated.");
            }

            var _fundsAuthorizationService = (IFundsAuthorizationService)fundsAuthorizationServiceResult;

            bool canUserAccessIfa = _fundsAuthorizationService.DoesUserHaveAccessToIfasAsync(ifaResult.ToArray()).GetAwaiter().GetResult();
            if (canUserAccessIfa == false)
            {
                throw new AdfsAuthException($"User does not have access to ifa {this.IfaCodeParameterName}");
            }
        }
    }
}
