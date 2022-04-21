using AdfsAuthenticationHandler.DI.Models;
using AdfsAuthenticationHandler.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace AdfsAuthenticationHandler.DI
{
    public  static class AdfsRoleRegistration
    {
        public static AdfsAuthBuilder AddRoles(this AdfsAuthBuilder adfsAuthBuilder, Action<RoleCheckerConfig> getRoleCheckerConfig = null)
        {
            var roleCheckConfig = new RoleCheckerConfig();
            if (getRoleCheckerConfig != null)
            {
                getRoleCheckerConfig(roleCheckConfig);
            }

            adfsAuthBuilder.Services.AddSingleton(roleCheckConfig);

            adfsAuthBuilder.Services.AddTransient<IAdfsUserProviderService, AdfsUserProviderService>();
            adfsAuthBuilder.Services.AddTransient<IUserAccessService, UserAccessService>();
            adfsAuthBuilder.Services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureRoleCheckerMvcOptions>();
            adfsAuthBuilder.Services.AddTransient<IUserRoleAuthorizationService, UserRoleAuthorizationService>();

            return adfsAuthBuilder;
        }
    }
}
