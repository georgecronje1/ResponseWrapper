using Microsoft.AspNetCore.Http;

namespace ResponseWrapper.Client.Models
{
    public class StandardResponse<T>
    {
        public ResponseType ResponseType { get; set; }
        public string Message { get; set; }
        public ValidationError[] ValidationErrors { get; set; }
        public T Result { get; set; }
        public Error Error { get; set; }

        public StandardResponse(ResponseType responseType, string message, ValidationError[] validationErrors, T result, Error error)
        {
            ResponseType = responseType;
            Message = message;
            ValidationErrors = validationErrors;
            Result = result;
            Error = error;
        }

        public StandardResponse()
        {
        }

        public static StandardResponse<T> MakeForError<T>(string errorMessage, object errorObject)
        {
            var error = new Error() { ErrorMessage = errorMessage, ErrorResult = errorObject };

            return new StandardResponse<T>(ResponseType.ExceptionError, "An exception has occurred when making the call.", new ValidationError[0], default(T), error);
        }
    }

    public class Error
    {
        public string ErrorMessage { get; set; }
        public object ErrorResult { get; set; }
    }

    public class ValidationError
    {
        public string Key { get; set; }
        public string Message { get; set; }
    }
}
