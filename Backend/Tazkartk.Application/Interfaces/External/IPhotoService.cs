using Microsoft.AspNetCore.Http;

namespace Tazkartk.Application.Interfaces
{
    public interface IPhotoService
    {
        Task<string>  AddPhotoAsync(IFormFile file);
        Task  DeletePhotoAsync(string PublicId);
    }
}
