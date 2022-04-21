using System;

namespace ResponseWrapper.Core.Exceptions
{
    /// <summary>
    /// Represents an error due to Unauthoirsed access
    /// </summary>
    public class UnauthorisedException : Exception
    {
        public UnauthorisedException() : base("You are unauthorised to use this service.")
        {
        }

        public UnauthorisedException(string message) : base(message)
        {
        }
    }
}
