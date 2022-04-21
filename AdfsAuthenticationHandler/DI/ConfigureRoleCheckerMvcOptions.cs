using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AdfsAuthenticationHandler.DI
{
    public class ConfigureRoleCheckerMvcOptions : IConfigureOptions<MvcOptions>
    {
        public void Configure(MvcOptions options)
        {
            options.Filters.Add<Filters.RoleCheckerFilter>();
        }
    }
}
