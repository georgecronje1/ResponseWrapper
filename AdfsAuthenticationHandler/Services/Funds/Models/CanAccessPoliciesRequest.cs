using System.Collections.Generic;

namespace AdfsAuthenticationHandler.Services.Funds.Models
{
    public class CanAccessPoliciesRequest
    {
        public List<string> PolicyNumbers { get; }
        public CanAccessPoliciesRequest(List<string> policyNumbers)
        {
            PolicyNumbers = policyNumbers;
        }
    }
}
