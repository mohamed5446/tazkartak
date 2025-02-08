using CloudinaryDotNet.Actions;

namespace Tazkartk.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string PublicId);
    }
}
