using Tazkartk.Models.Enums;

namespace Tazkartk.DTO.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string message { get; set; }
        public T? Data { get; set; }
        public StatusCode StatusCode { get; set; }

    }
}
