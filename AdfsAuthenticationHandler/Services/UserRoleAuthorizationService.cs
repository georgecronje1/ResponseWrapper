using AdfsAuthenticationHandler.DI.Models;
using AdfsAuthenticationHandler.Services.Funds.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AdfsAuthenticationHandler.Services
{
    public interface IUserRoleAuthorizationService
    {
        Task<HttpResponseMessage> AuthorizationPostAsync(string method, object data);
    }
    public class UserRoleAuthorizationService : IUserRoleAuthorizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserRoleAuthorisationConfiguration _userRoleAuthorisationConfiguration;

        public UserRoleAuthorizationService(IHttpContextAccessor httpContextAccessor, UserRoleAuthorisationConfiguration userRoleAuthorisationConfiguration)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRoleAuthorisationConfiguration = userRoleAuthorisationConfiguration;
        }
        private string GetApplicationAuthorizationHeader()
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization") == false ||
                string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString()))
            {
                return string.Empty;
            }
            var returnValue = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (returnValue.ToUpperInvariant().StartsWith("BEARER"))
            {
                returnValue = returnValue.Substring("BEARER".Length);
            }
            return returnValue.Trim();
        }
        private string GetUserAuthorizationHeader()
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("x-api-token") == false ||
                string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext.Request.Headers["x-api-token"].ToString()))
            {
                return string.Empty;
            }
            return _httpContextAccessor.HttpContext.Request.Headers["x-api-token"].ToString();
        }

        public async Task<HttpResponseMessage> AuthorizationPostAsync(string method, object data)
        {
            var url = BuilUrl(method);
            
            var client = new HttpClient();
            var applicationToken = GetApplicationAuthorizationHeader();
            var userToken = GetUserAuthorizationHeader();

            if (string.IsNullOrWhiteSpace(applicationToken) || string.IsNullOrWhiteSpace(userToken))
                throw new Exception("AUTH EXCEPTION");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", applicationToken);
            client.DefaultRequestHeaders.Add("x-api-token", userToken);
            var jsonRequest = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content).ConfigureAwait(false);

            return response;
        }

        private string BuilUrl(string method)
        {
            return _userRoleAuthorisationConfiguration.UserAuthorisationEndpoint.EndsWith('/') ?
                   _userRoleAuthorisationConfiguration.UserAuthorisationEndpoint + method :
                   _userRoleAuthorisationConfiguration.UserAuthorisationEndpoint + "/" + method;
        }

    }
}
