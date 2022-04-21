using System;

namespace ResponseWrapper.Core.Exceptions
{
    /// <summary>
    /// Represents an error due to bad input
    /// </summary>
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
