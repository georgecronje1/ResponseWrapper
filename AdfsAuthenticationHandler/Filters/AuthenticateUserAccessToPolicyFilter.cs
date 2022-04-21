using AdfsAuthenticationHandler.Exceptions;
using AdfsAuthenticationHandler.Services.Funds;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace AdfsAuthenticationHandler.Filters
{
    public class AuthenticateUserAccessToPolicyFilter : ActionFilterAttribute
    {
        public string ContractNumberParameterName { get; set; }

        public AuthenticateUserAccessToPolicyFilter()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrWhiteSpace(this.ContractNumberParameterName))
            {
                throw new System.Exception($"ContractNumberParameterName number cannot be empty");
            }

            var contractNumber = context.ActionArguments.SingleOrDefault(a => a.Key == this.ContractNumberParameterName);

            if (string.IsNullOrWhiteSpace(contractNumber.Value?.ToString()))
            {
                throw new System.Exception($"Contract Number could not be found for parameter name: {this.ContractNumberParameterName}");
            }

            var isEnumberable = contractNumber.Value as IEnumerable<string> != null;
            var contractNumberResult = isEnumberable ? (List<string>)contractNumber.Value : new List<string>() { contractNumber.Value.ToString() };

            var fundsAuthorizationServiceResult = context.HttpContext.RequestServices.GetService(typeof(IFundsAuthorizationService));
            if (fundsAuthorizationServiceResult == null)
            {
                throw new System.Exception($"Fund authorization service not instantiated.");
            }

            var _fundsAuthorizationService = (IFundsAuthorizationService)fundsAuthorizationServiceResult;

            bool canUserAccessContract = _fundsAuthorizationService.DoesUserHaveAccessToPoliciesAsync(contractNumberResult.ToArray()).GetAwaiter().GetResult();
            if (canUserAccessContract == false)
            {
                throw new AdfsAuthException($"User does not have access to ifa {this.ContractNumberParameterName}");
            }
        }
    }
}
 