using Microsoft.AspNetCore.Http;

namespace Tazkartk.Application.Interfaces.External
{
    public interface IPhotoService
    {
        Task<string> AddPhotoAsync(IFormFile file);
        Task DeletePhotoAsync(string PublicId);
    }
}
