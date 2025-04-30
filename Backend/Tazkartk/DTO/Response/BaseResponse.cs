namespace Tazkartk.DTO.Response
{
   public class BaseResponse
{
    public List<string> Errors { get; set; }

    public BaseResponse(List<string> errors)
    {
        Errors = errors;
    }
}
}
