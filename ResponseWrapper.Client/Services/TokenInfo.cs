using System;

namespace ResponseWrapper.Client.Services
{
    public class TokenInfo
    {
        public string Token { get; set; }
        public DateTime? Expires { get; set; }
    }
}
