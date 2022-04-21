namespace ResponseWrapper.Client.Models
{
    public class GenericResponse<T>
    {
        public bool IsSuccess { get; }

        public T Data { get; }

        public GenericErrorResult ErrorResult { get; }

        GenericResponse(bool isSuccess, T data, GenericErrorResult errorResult)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorResult = errorResult;
        }

        static GenericResponse<T1> Make<T1>(bool isSuccess, T1 data, GenericErrorResult errorResult)
        {
            return new GenericResponse<T1>(isSuccess, data, errorResult);
        }

        public static GenericResponse<T1> MakeSuccess<T1>(T1 data)
        {
            return Make<T1>(true, data, GenericErrorResult.Make(string.Empty, "200"));
        }

        public static GenericResponse<T1> MakeError<T1>(GenericErrorResult errorResult)
        {
            return Make<T1>(false, default(T1), errorResult);
        }

        public static GenericResponse<T1> MakeErrorWithResult<T1>(GenericErrorResult errorResult)
        {
            return Make<T1>(false, default(T1), errorResult);
        }
    }
}
