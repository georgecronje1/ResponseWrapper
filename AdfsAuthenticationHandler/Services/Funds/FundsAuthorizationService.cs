using AdfsAuthenticationHandler.Services.Funds.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AdfsAuthenticationHandler.Services.Funds
{
    public interface IFundsAuthorizationService
    {
        Task<bool> DoesUserHaveAccessToIfasAsync(params string[] ifaCodes);
        Task<bool> DoesUserHaveAccessToPoliciesAsync(params string[] policies);
    }

    public class FundsAuthorizationService : IFundsAuthorizationService
    {
        readonly IUserRoleAuthorizationService _userRoleAuthorizationService;

        public FundsAuthorizationService(IUserRoleAuthorizationService userRoleAuthorizationService)
        {
            _userRoleAuthorizationService = userRoleAuthorizationService;
        }

        const string _CanAccessIfaPath = "Authorize/CanAccessIfas";
        const string _CanAccessPoliciesPath = "Authorize/CanAccessPolicies";

        public async Task<bool> DoesUserHaveAccessToIfasAsync(params string[] ifaCodes)
        {
            if (ifaCodes == null || ifaCodes.Length == 0) return true;

            var request = new CanAccessIfaRequest(ifaCodes.ToList());
            var response = await _userRoleAuthorizationService.AuthorizationPostAsync(_CanAccessIfaPath, request).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DoesUserHaveAccessToPoliciesAsync(params string[] policies)
        {
            if (policies == null || policies.Length == 0) return true;

            var request = new CanAccessPoliciesRequest(policies.ToList());
            var response = await _userRoleAuthorizationService.AuthorizationPostAsync(_CanAccessPoliciesPath, request).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }
    }
}
