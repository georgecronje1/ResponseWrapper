using AdfsAuthenticationHandler.Identities;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace AdfsAuthenticationHandler.Services
{
    public interface IAdfsUserProviderService
    {
        bool DoesUserHaveAnyOfTheseRoles(string[] roles);
        bool HasApplicationAndUserIdentity();
        bool IsUserLoggedIn();

    }

    public class AdfsUserProviderService : IAdfsUserProviderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdfsUserProviderService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        ClaimsPrincipal GetUser()
        {
            return _httpContextAccessor.HttpContext.User as ClaimsPrincipal;
        }

        public bool IsUserLoggedIn()
        {
            var principal = GetUser();

            return principal.IsMainIdentityLoggedIn();
        }

        public bool HasApplicationAndUserIdentity()
        {
            if (IsUserLoggedIn() == false)
            {
                return false;
            }

            var principal = GetUser();

            return principal.HasAtleastTwoLoggedIn() && principal.HasAtleastOneUserClaimsIdentity();

        }

        public bool DoesUserHaveAnyOfTheseRoles(string[] roles)
        {
            var principal = GetUser();

            var isInRightRole = false;
            roles.ToList().ForEach(r =>
            {
                var isInRole = principal.IsInRole(r);
                if (isInRole)
                {
                    isInRightRole = true;
                }
            });

            return isInRightRole;
        }
    }

    public static class IdentityExtension
    {
        public static bool IsMainIdentityLoggedIn(this ClaimsPrincipal principal)
        {
            return principal != null && principal.Identity.IsAuthenticated;
        }

        public static bool AreAllIdentitiesLoggedIn(this ClaimsPrincipal principal)
        {
            if (principal.IsMainIdentityLoggedIn())
            {
                return principal.Identities.Count(i => i.IsAuthenticated) > 0;
            }

            return false;
        }

        public static bool HasAtleastTwoLoggedIn(this ClaimsPrincipal principal)
        {
            if (principal.AreAllIdentitiesLoggedIn())
            {
                return principal.Identities.Count(i => i.IsAuthenticated) > 1;
            }

            return false;
        }

        public static bool HasAtleastOneUserClaimsIdentity(this ClaimsPrincipal principal)
        {
            if (principal.HasAtleastTwoLoggedIn())
            {
                return principal.Identities.OfType<UserClaimsIdentity>().Count() > 0;
            }

            return false;
        }
    }
}
