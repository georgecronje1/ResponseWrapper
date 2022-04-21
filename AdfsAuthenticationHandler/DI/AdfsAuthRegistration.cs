using AdfsAuthenticationHandler.DI.Models;
using AdfsAuthenticationHandler.Identities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AdfsAuthenticationHandler.DI
{
    public static class AdfsAuthRegistration
    {
        public static AdfsAuthBuilder AddAdfsAuth(this IServiceCollection services, Action<AdfsConfig> getAdfsConfig)
        {
            var adfsConfig = new AdfsConfig();
            getAdfsConfig(adfsConfig);
            services.AddSingleton(adfsConfig);

            var userAuthorisationConfiguration = new UserRoleAuthorisationConfiguration(adfsConfig.UserAuthorizationEndpoint.ToString());
            services.AddSingleton(userAuthorisationConfiguration);

            services.AddHttpContextAccessor();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
                options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(adfsConfig.AuthTokenEndpoint.ToString(), new OpenIdConnectConfigurationRetriever())
                {
                    RefreshInterval = new TimeSpan(6, 0, 0)
                };
                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = async context => await ApplicationTokenValidatedAsync(context).ConfigureAwait(false)
                };
            });
            return new AdfsAuthBuilder(services);
        }

        /// <summary>
        /// Application token was validate, see if we can validate the user token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task ApplicationTokenValidatedAsync(TokenValidatedContext context)
        {
            if (context.HttpContext.Request.Headers.ContainsKey("x-api-token") == false ||
               string.IsNullOrEmpty(context.HttpContext.Request.Headers["x-api-token"]))
            {
                return;
            }
            var token = context.HttpContext.Request.Headers["x-api-token"].ToString();
            var validationResult = await  AuthenticateUserAsync(context, token).ConfigureAwait(false);
            if (validationResult.Succeeded && validationResult.Principal != null)
            {
                var userIdentities = validationResult.Principal.Identities.Select(s => new UserClaimsIdentity(s));
                context.Principal.AddIdentities(userIdentities);
            }
        }
        private static async Task<AuthenticateResult> AuthenticateUserAsync(TokenValidatedContext context, string token)
        {
            var configuration = await context.Options.ConfigurationManager.GetConfigurationAsync(CancellationToken.None).ConfigureAwait(false);
            var validator = context.Options.SecurityTokenValidators.Single();   //There should be only one, if 0 or > 1 then we must get an exception here
            if (validator.CanReadToken(token) == false)
            {
                return AuthenticateResult.Fail("Invalid token");
            }

            SecurityToken securityToken = null;
            ClaimsPrincipal claimsPrincipal = null;
            var tokenValidationParameters = context.Options.TokenValidationParameters.Clone();
            tokenValidationParameters.IssuerSigningKeys = configuration.SigningKeys;
            try
            {
                claimsPrincipal = validator.ValidateToken(token, tokenValidationParameters, out securityToken);
            }
            catch (Exception exception)
            {
                return AuthenticateResult.Fail(exception);
            }

            var ticket = new AuthenticationTicket(claimsPrincipal, new AuthenticationProperties(), JwtBearerDefaults.AuthenticationScheme);
            return AuthenticateResult.Success(ticket);
        }



        public static void UseAdfsAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}
