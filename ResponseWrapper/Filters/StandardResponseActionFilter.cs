using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ResponseWrapper.DI;
using ResponseWrapper.Models;
using System.Collections.Generic;

namespace ResponseWrapper.Filters
{
    public class StandardResponseActionFilter : IActionFilter
    {
        readonly ResponseConfig _config;

        public StandardResponseActionFilter(ResponseConfig config)
        {
            _config = config;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                var errors = new List<ValidationErrors>();

                foreach (var key in context.ModelState.Keys)
                {
                    ModelStateEntry errorValue;
                    if (context.ModelState.TryGetValue(key, out errorValue))
                    {
                        foreach (var error in errorValue.Errors)
                        {
                            errors.Add(new ValidationErrors { Key = key, Message = error.ErrorMessage });
                        }
                    }
                }

                var standardResponse = StandardResponse.MakeValidationErrors(errors.ToArray());

                context.Result = new JsonResult(standardResponse)
                {
                    StatusCode = 500
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var result = context.Result;

            if (result == null)
            {
                return;
            }
            var Item = _config.GetConfig(result);

            var standardResponse = StandardResponse.MakeSuccessWithFriendlyMessage(Item.DefaultMessage);

            switch (Item.ResultType)
            {
                case ResultType.Ok:
                case ResultType.Json:
                    if (string.IsNullOrWhiteSpace(Item.ResultMessage) == false)
                    {
                        standardResponse = StandardResponse.MakeSuccess(Item.DefaultMessage, Item.ResultMessage);
                    }
                    else
                    {
                        standardResponse = StandardResponse.MakeSuccess(Item.DefaultMessage, Item.ResultObject);
                    }
                    break;
                case ResultType.NotFound:
                    if (string.IsNullOrWhiteSpace(Item.ResultMessage) == false)
                    {
                        standardResponse = StandardResponse.MakeNotFound(Item.DefaultMessage, Item.ResultMessage);
                    }
                    else if (Item.ResultObject != null)
                    {
                        standardResponse = StandardResponse.MakeNotFound(Item.DefaultMessage, Item.ResultObject);
                    }
                    else
                    {
                        standardResponse = StandardResponse.MakeNotFoundWithFriendlyMessage(Item.DefaultMessage);
                    }
                    break;
                case ResultType.BadRequest:
                    if (string.IsNullOrWhiteSpace(Item.ResultMessage) == false)
                    {
                        standardResponse = StandardResponse.MakeBadRequest(Item.DefaultMessage, Item.ResultMessage);
                    }
                    else if (Item.ResultObject != null)
                    {
                        standardResponse = StandardResponse.MakeBadRequest(Item.DefaultMessage, Item.ResultObject);
                    }
                    else
                    {
                        standardResponse = StandardResponse.MakeBadRequestWithFriendlyMessage(Item.DefaultMessage);
                    }
                    break;
                case ResultType.Unknown:
                default:
                    break;
            }

            context.Result = new JsonResult(standardResponse)
            {
                StatusCode = Item.StatusCode
            };
        }
    }
}
