using Microsoft.AspNetCore.Mvc;

namespace ResponseWrapper.DI
{
    public class ResponseConfig
    {
        ResponseConfigItemBase OkItem { get; set; } = ResponseConfigItemBase.MakeDefault(ResultType.Ok, 200, "Request completed successfully");
        ResponseConfigItemBase JsonItem { get; set; } = ResponseConfigItemBase.MakeDefault(ResultType.Json, 200, "Request completed successfully");
        ResponseConfigItemBase NotFoundItem { get; set; } = ResponseConfigItemBase.MakeDefault(ResultType.NotFound, 404, "Requested resource could not be found");
        ResponseConfigItemBase BadItem { get; set; } = ResponseConfigItemBase.MakeDefault(ResultType.BadRequest, 500, "Something went wrong");
        ResponseConfigItemBase DefaultItem { get; set; } = ResponseConfigItemBase.MakeDefault(ResultType.Json, 200, "Request completed successfully");

        public void SetOkResponseDefaultMessage(string newMessage)
        {
            OkItem = ResponseConfigItemBase.MakeDefault(ResultType.Ok, 200, newMessage);
        }

        public void SetNotFoundResponseDefaultMessage(string newMessage)
        {
            NotFoundItem = ResponseConfigItemBase.MakeDefault(ResultType.NotFound, 404, newMessage);
        }

        public ResponseConfigItemBase GetConfig(IActionResult result)
        {
            var resultType = GetResultType(result);
            var item = GetItem(resultType);

            var hasValue = result.GetType().IsSubclassOf(typeof(ObjectResult));
            if (hasValue)
            {
                var value = (result as ObjectResult).Value;
                if (value is string stringValue)
                {
                    item = ResponseConfigItemBase.MakeWithResultMessage(item, stringValue);
                }
                else
                {
                    item = ResponseConfigItemBase.MakeWithResulObject(item, value);
                }
            }

            return item;
        }

        ResponseConfigItemBase GetItem(ResultType resultType)
        {
            switch (resultType)
            {
                case ResultType.Ok:
                    return OkItem;
                case ResultType.Json:
                    return JsonItem;
                case ResultType.NotFound:
                    return NotFoundItem;
                case ResultType.BadRequest:
                    return BadItem;
                case ResultType.Unknown:
                default:
                    return DefaultItem;
            }
        }

        ResultType GetResultType(IActionResult result)
        {
            if (result is OkResult || result is OkObjectResult)
            {
                return ResultType.Ok;
            }

            if (result is JsonResult)
            {
                return ResultType.Json;
            }

            if (result is NotFoundResult || result is NotFoundObjectResult)
            {
                return ResultType.NotFound;
            }

            if (result is BadRequestResult || result is BadRequestObjectResult)
            {
                return ResultType.BadRequest;
            }

            return ResultType.Unknown;
        }
    }

    public class ResponseConfigItemBase
    {
        public ResultType ResultType { get; }

        public int StatusCode { get; }

        public string DefaultMessage { get; }

        public string ResultMessage { get; }

        public object ResultObject { get; }

        ResponseConfigItemBase(ResultType resultType, int statusCode, string defaultMessage, string resultMessage, object resultObject)
        {
            ResultType = resultType;
            StatusCode = statusCode;
            DefaultMessage = defaultMessage;
            ResultMessage = resultMessage;
            ResultObject = resultObject;
        }

        public static ResponseConfigItemBase Make(ResultType resultType, int statusCode, string defaultMessage, string resultMessage, object resultObject)
        {
            return new ResponseConfigItemBase(resultType, statusCode, defaultMessage, resultMessage, resultObject);
        }

        public static ResponseConfigItemBase MakeDefault(ResultType resultType, int statusCode, string defaultMessage)
        {
            return new ResponseConfigItemBase(resultType, statusCode, defaultMessage, resultMessage: string.Empty, resultObject: null);
        }

        public static ResponseConfigItemBase MakeWithResultMessage(ResponseConfigItemBase item, string resultMessage)
        {
            return Make(item.ResultType, item.StatusCode, item.DefaultMessage, resultMessage, resultObject: null);
        }

        public static ResponseConfigItemBase MakeWithResulObject(ResponseConfigItemBase item, object resultObject)
        {
            return Make(item.ResultType, item.StatusCode, item.DefaultMessage, resultMessage: string.Empty, resultObject: resultObject);
        }
    }

    public enum ResultType
    {
        Unknown,
        Ok,
        Json,
        NotFound,
        BadRequest
    }
}
