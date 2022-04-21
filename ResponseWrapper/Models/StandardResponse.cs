using System.Collections.Generic;

namespace ResponseWrapper.Models
{
    public class StandardResponse<T>
    {
        /// <summary>
        /// String to show if request was successful or resulted in an error (validation or other type)
        /// </summary>
        public ResponseType ResponseType { get; }

        /// <summary>
        /// User Friendly message that can always be shown to the end user
        /// </summary>
        public string Message { get; }

        public ValidationErrors[] ValidationErrors { get; }

        public T Result { get; }


        public ErrorDetails Error { get; set; }

        protected StandardResponse(ResponseType responseType, string message, ValidationErrors[] validationErrors, T result, ErrorDetails error)
        {
            ResponseType = responseType;
            Message = message;
            ValidationErrors = validationErrors;
            Result = result;
            Error = error;
        }

        public static StandardResponse<T> MakeSuccessWithFriendlyMessage(string message)
        {
            return new StandardResponse<T>(ResponseType.Success, message, new Models.ValidationErrors[0], default(T), default(ErrorDetails));
        }

        public static StandardResponse<T> MakeSuccess(string message, T result)
        {
            return new StandardResponse<T>(ResponseType.Success, message, new Models.ValidationErrors[0], result, default(ErrorDetails));
        }

        public static StandardResponse<T> MakeNotFoundWithFriendlyMessage(string message)
        {
            return new StandardResponse<T>(ResponseType.NotFound, message, new Models.ValidationErrors[0], default(T), default(ErrorDetails));
        }

        public static StandardResponse<T> MakeException(string errorMessage)
        {
            return new StandardResponse<T>(ResponseType.ExceptionError, errorMessage, new Models.ValidationErrors[0], default(T), default(ErrorDetails));
        }

        public static StandardResponse<T> MakeException(object errorMessage)
        {
            return new StandardResponse<T>(ResponseType.ExceptionError, string.Empty, new Models.ValidationErrors[0], default(T), ErrorDetails.Make(errorMessage));
        }

        public static StandardResponse<T> MakeValidationErrors(Models.ValidationErrors[] validationErrors, string errorMessage = "Validation Errors")
        {
            return new StandardResponse<T>(ResponseType.ValidationError, errorMessage, validationErrors, default(T), default(ErrorDetails));
        }
    }

    public class StandardResponse : StandardResponse<object>
    {
        StandardResponse(ResponseType responseType, string message, ValidationErrors[] validationErrors, object result, ErrorDetails errorDetails) : base(responseType, message, validationErrors, result, errorDetails)
        {

        }

        public static StandardResponse MakeNotFound(string message, string result)
        {
            return new StandardResponse(ResponseType.NotFound, message, new Models.ValidationErrors[0], default, ErrorDetails.MakeWithMessage(result));
        }

        public static StandardResponse MakeNotFound(string message, object result)
        {
            return new StandardResponse(ResponseType.NotFound, message, new Models.ValidationErrors[0], default, ErrorDetails.Make(result));
        }

        public static StandardResponse MakeBadRequest(string message, string result)
        {
            return new StandardResponse(ResponseType.ExceptionError, message, new Models.ValidationErrors[0], default, ErrorDetails.MakeWithMessage(result));
        }

        public static StandardResponse MakeBadRequest(string message, object result)
        {
            return new StandardResponse(ResponseType.ExceptionError, message, new Models.ValidationErrors[0], default, ErrorDetails.Make(result));
        }

        public static StandardResponse MakeBadRequestWithFriendlyMessage(string message)
        {
            return new StandardResponse(ResponseType.ExceptionError, message, new Models.ValidationErrors[0], default, default(ErrorDetails));
        }
    }

    public class ResponseType
    {
        public string Description { get; }

        ResponseType(string description)
        {
            Description = description;
        }

        public static ResponseType Success = new ResponseType("Success");
        public static ResponseType NotFound = new ResponseType("NotFound");
        public static ResponseType ExceptionError = new ResponseType("ExceptionError");
        public static ResponseType ValidationError = new ResponseType("ValidationError");

        public static IEnumerable<ResponseType> All = new ResponseType[]
        {
            Success,
            NotFound,
            ExceptionError,
            ValidationError
        };
    }

    public class ValidationErrors
    {
        public string Key { get; set; }

        public string Message { get; set; }
    }

    public class ErrorDetails
    {
        public string ErrorMessage { get; }

        public object ErrorResult { get; }

        ErrorDetails(string errorMessage, object errorResult)
        {
            ErrorMessage = errorMessage;
            ErrorResult = errorResult;
        }

        internal static ErrorDetails Make(object errorMessage)
        {
            return new ErrorDetails(string.Empty, errorMessage);
        }

        internal static ErrorDetails MakeWithMessage(string result)
        {
            return new ErrorDetails(result, null);
        }
    }
}
