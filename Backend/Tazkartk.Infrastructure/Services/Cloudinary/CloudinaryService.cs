using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Tazkartk.Infrastructure.Helpers;
using Tazkartk.Application.Interfaces.External;

namespace Tazkartk.Infrastructure.Cloudinary
{
    public class CloudinaryService:IPhotoService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
            _cloudinary = new CloudinaryDotNet.Cloudinary(acc);
        }
        public async Task<string> AddPhotoAsync(IFormFile file)
        {
            //   _cloudinaryService.AddPhotoAsync(file); 
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/webp" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                    throw new ArgumentException("الملف المرفوع يجب أن يكون صورة فقط.");
                using Stream stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if(uploadResult.Error != null) throw new Exception("حدث خطأ أثناء رفع الصورة: " + uploadResult.Error.Message);
                return uploadResult.Url.ToString();
            }
            throw new ArgumentException("الصورة غير صالحة أو فارغة.");
        }


        public async Task DeletePhotoAsync(string PublicId)
        {
            //  _cloudinaryService.DeletePhotoAsync(PublicId);
            var deleteParams = new DeletionParams(PublicId);
            var Result = await _cloudinary.DestroyAsync(deleteParams);
            if(Result.Error != null) throw new Exception("حدث خطأ أثناء حذف الصورة: " + Result.Error.Message);
        }
    }
}
