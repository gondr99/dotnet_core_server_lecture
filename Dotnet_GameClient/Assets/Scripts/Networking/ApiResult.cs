namespace Networking
{
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public long StatusCode { get; set; }
        public ApiError Error { get; set; }

        public static ApiResult<T> Success(T data, long statusCode) => new ApiResult<T>
        {
            IsSuccess = true, 
            Data = data, 
            StatusCode = statusCode
        };

        public static ApiResult<T> Fail(ApiError error, long statusCode) => new ApiResult<T>
        {
            IsSuccess = false,
            Error = error,
            StatusCode = statusCode
        };
        
        
    }
}

