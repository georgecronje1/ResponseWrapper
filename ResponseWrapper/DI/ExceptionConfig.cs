using ResponseWrapper.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ResponseWrapper.DI
{
    public class ExceptionConfig
    {
        public string DefaultMessage { get; private set; }

        public List<ExceptionItem> ExceptionItems { get; private set; } = new List<ExceptionItem>();

        public ExceptionItem GetExceptionItem(Exception ex)
        {
            var configItem = ExceptionItems.FirstOrDefault(i => i.ExceptionType == ex.GetType());
            if (configItem == null)
            {
                return new ExceptionItem
                {
                    ShowExceptionMessage = false,
                    HasMessageHandler = false,
                    ExceptionType = ex.GetType()
                };
            }

            return configItem;
        }

        public void SetDefaultMessage(string message)
        {
            DefaultMessage = message;
        }

        public void AddExceptionItem<T>(bool showExceptionMessage = false, int statusCode = 500, bool hasMessageHandler = false, Func<Exception, string> messageHandler = null)
        {
            var item = new ExceptionItem { ExceptionType = typeof(T), ShowExceptionMessage = showExceptionMessage, StatusCode = statusCode, HasMessageHandler = hasMessageHandler, MessageHandler = messageHandler };
            ExceptionItems.Add(item);
        }

        public void AddExceptionItemWithShowMessage<T>()
        {
            var item = new ExceptionItem { ExceptionType = typeof(T), ShowExceptionMessage = true, StatusCode = 500, HasMessageHandler = false, MessageHandler = null };
            ExceptionItems.Add(item);
        }

        public void AddExceptionItemWithMessageHandler<T>(Func<Exception, string> messageHandler = null)
        {
            var item = new ExceptionItem { ExceptionType = typeof(T), ShowExceptionMessage = true, StatusCode = 500, HasMessageHandler = true, MessageHandler = messageHandler };
            ExceptionItems.Add(item);
        }

        /// <summary>
        /// Catches the 'Standard Exceptions' and responds with the following status codes:
        /// <list type="bullet">
        /// <item><see cref="EntityNotFoundException"/> -> (404) <see cref="HttpStatusCode.NotFound"/></item>
        /// <item><see cref="BadRequestException"/>     -> (400) <see cref="HttpStatusCode.BadRequest"/></item>
        /// <item><see cref="BusinessRuleException"/>   -> (409) <see cref="HttpStatusCode.Conflict"/></item>
        /// <item><see cref="UnauthorisedException"/>   -> (401) <see cref="HttpStatusCode.Unauthorized"/></item>
        /// <item><see cref="RestException"/>           -> (500) <see cref="HttpStatusCode.InternalServerError"/></item>
        /// </list>
        /// </summary>
        public void UseStandardExceptions()
        {
            var entityNotFound = new ExceptionItem(HttpStatusCode.NotFound, typeof(EntityNotFoundException));
            var badRequest = new ExceptionItem(HttpStatusCode.BadRequest, typeof(BadRequestException));
            var businessRule = new ExceptionItem(HttpStatusCode.Conflict, typeof(BusinessRuleException));
            var unauthorised = new ExceptionItem(HttpStatusCode.Unauthorized, typeof(UnauthorisedException));
            var rest = new ExceptionItem(HttpStatusCode.InternalServerError, typeof(RestException));

            ExceptionItems.AddRange(new ExceptionItem[] { entityNotFound, badRequest, businessRule, unauthorised, rest });
        }
    }
}
