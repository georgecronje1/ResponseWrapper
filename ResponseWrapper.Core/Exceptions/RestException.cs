using System;
using System.Net;

namespace ResponseWrapper.Core.Exceptions
{
    /// <summary>
    /// Represents a generic error thrown during an HTTP request
    /// </summary>
    public class RestException : Exception
    {
        /// <summary>
        /// HTTP Status code received when making the call
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; private set; }

        public RestException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public RestException(HttpStatusCode httpStatusCode, string message, Exception exception) : base(message, exception)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
