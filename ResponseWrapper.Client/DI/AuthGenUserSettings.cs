using System;
using System.Collections.Generic;
using System.Text;

namespace ResponseWrapper.Client.DI
{
    public class AuthGenUserSettings
    {
        public string OAuthURL { get; set; }
        public Oauth2TokenConfig.ApplicationTokenSettings UserTokenSettings { get; set; }

        AuthGenUserSettings(string oAuthURL, Oauth2TokenConfig.ApplicationTokenSettings userTokenSettings)
        {
            OAuthURL = oAuthURL;
            UserTokenSettings = userTokenSettings;
        }

        public static AuthGenUserSettings Make(string oAuthURL, Oauth2TokenConfig.ApplicationTokenSettings userTokenSettings)
        {
            return new AuthGenUserSettings(oAuthURL, userTokenSettings);
        }
    }
}
