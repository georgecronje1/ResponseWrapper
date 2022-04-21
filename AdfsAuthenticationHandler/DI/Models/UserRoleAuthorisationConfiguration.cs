namespace AdfsAuthenticationHandler.DI.Models
{
    public class UserRoleAuthorisationConfiguration
    {
        public string UserAuthorisationEndpoint { get;  }

        public UserRoleAuthorisationConfiguration(string userAuthorisationEndpoint)
        {
            UserAuthorisationEndpoint = userAuthorisationEndpoint;
        }
    }
}
