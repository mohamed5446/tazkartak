using Tazkartk.Application;

namespace Tazkartk.Application.DTO.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string message { get; set; }
        public T? Data { get; set; }
        public StatusCode StatusCode { get; set; }

        public ApiResponse(bool success, string message, T? data, StatusCode statusCode)
        {
            Success = success;
            this.message = message;
            Data = data;
            StatusCode = statusCode;
        }
        public ApiResponse(bool success, string message, StatusCode statusCode)
        {
            Success = success;
            this.message = message;
            StatusCode = statusCode;
        }
        public static ApiResponse<T> success(string message, StatusCode statusCode = StatusCode.Ok)
        {
            return new ApiResponse<T>(true, message, default, statusCode);
        }
        public static ApiResponse<T> success(string message, T? data, StatusCode statusCode = StatusCode.Ok)
        {
            return new ApiResponse<T>(true, message, data, statusCode);
        }
        public static ApiResponse<T> Error(string message, StatusCode statusCode = StatusCode.BadRequest)
        {
            return new ApiResponse<T>(false, message, statusCode);
        }

    }
}
