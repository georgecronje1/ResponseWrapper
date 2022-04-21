using System;
using System.Net;

namespace ResponseWrapper.DI
{
    public class ExceptionItem
    {
        public bool ShowExceptionMessage { get; set; }

        public bool HasMessageHandler { get; set; }

        public int StatusCode { get; set; } = 500;

        public Type ExceptionType { get; set; }

        public Func<Exception, string> MessageHandler { get; set; }

        public ExceptionItem()
        {

        }

        public ExceptionItem(HttpStatusCode statusCode, Type exceptionType)
        {
            ShowExceptionMessage = true;
            HasMessageHandler = false;
            MessageHandler = null;
            StatusCode = (int)statusCode;
            ExceptionType = exceptionType;
        }
    }
}
