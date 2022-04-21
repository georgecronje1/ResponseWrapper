using System.Collections.Generic;
using System.Security.Claims;

namespace AdfsAuthenticationHandler.Identities
{
    public class UserClaimsIdentity : ClaimsIdentity
    {
        public UserClaimsIdentity(ClaimsIdentity claimsIdentity) : base(claimsIdentity)
        {
            
        }
    }
}
