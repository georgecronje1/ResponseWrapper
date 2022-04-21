using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ResponseWrapper.DI;
using ResponseWrapper.Extensions;
using ResponseWrapper.Models;

namespace ResponseWrapper.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
	{
		readonly ExceptionConfig _exceptionConfig;
		readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ExceptionConfig exceptionConfig, ILogger<ApiExceptionFilter> logger)
        {
            _exceptionConfig = exceptionConfig;
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
		{
			var ex = context.Exception;

			var errorMessage = ex.ExtractExceptionDescription();
			_logger.LogError(message: $"REST Call to {context.HttpContext.Request.Path.Value} Resulted with error: {errorMessage}");

			var msg = _exceptionConfig.DefaultMessage;

			var configItem = _exceptionConfig.GetExceptionItem(ex);

			if (configItem.ShowExceptionMessage == false)
			{
				msg = _exceptionConfig.DefaultMessage;
			}

			if (configItem.ShowExceptionMessage && configItem.HasMessageHandler == false)
			{
				msg = ex.Message;
			}

			if (configItem.ShowExceptionMessage && configItem.HasMessageHandler)
			{
				var display = configItem.MessageHandler(ex);
				msg = display;
			}

			var standardResponse = StandardResponse.MakeException(msg);

			context.Result = new JsonResult(standardResponse)
			{
				StatusCode = configItem.StatusCode
			};

			base.OnException(context);
		}
	}
}
