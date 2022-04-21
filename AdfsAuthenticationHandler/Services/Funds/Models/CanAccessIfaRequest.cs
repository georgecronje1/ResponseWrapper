using System.Collections.Generic;

namespace AdfsAuthenticationHandler.Services.Funds.Models
{
    public class CanAccessIfaRequest
    {
        public List<string> IfaCodes { get; }
        public CanAccessIfaRequest(List<string> ifaCodes)
        {
            IfaCodes = ifaCodes;
        }
    }
}
