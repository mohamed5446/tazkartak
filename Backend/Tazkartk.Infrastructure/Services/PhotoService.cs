//using CloudinaryDotNet;
//using CloudinaryDotNet.Actions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Options;
//using Tazkartk.Application.DTO.CompanyDTOs;
//using Tazkartk.Application.DTO.Response;
//using Tazkartk.Application;
//using Tazkartk.Infrastructure.Cloudinary;


//namespace Tazkartk.Services
//{
//    public class PhotoService : IPhotoService
//    {
//        // private readonly Cloudinary _cloudinary;

//        private readonly ICloudinaryService _cloudinaryService;
//        //public PhotoService(IOptions<CloudinarySettings> config)
//        //{
//        //    var acc = new CloudinaryDotNet.Account(
//        //        config.Value.CloudName,
//        //        config.Value.ApiKey,
//        //        config.Value.ApiSecret
//        //        );
//        //    _cloudinary = new Cloudinary(acc);
//        //}

//        public async Task<bool> AddPhotoAsync(IFormFile file)
//        {
//           ImageUploadResult photoResult =await  _cloudinaryService.AddPhotoAsync(file);
//            if (photoResult.Error ==null)
//            {
//                return true;
//                //return ApiResponse<CompanyDTO>.Error($"حدث خطا اثناء تعديل الصورة:{photoResult.Error.Message}", StatusCode.InternalServerError);

//            }
//            return false;
//            //var uploadResult = new ImageUploadResult();
//            //if (file.Length > 0)
//            //{
//            //    var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/webp" };
//            //    if (!allowedTypes.Contains(file.ContentType.ToLower()))
//            //        throw new ArgumentException("الملف المرفوع يجب أن يكون صورة فقط.");
//            //    using Stream stream = file.OpenReadStream();
//            //    var uploadParams = new ImageUploadParams
//            //    {
//            //        File = new FileDescription(file.FileName, stream),
//            //        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
//            //    };

//            //    uploadResult = await _cloudinary.UploadAsync(uploadParams);
//            //}
//            //return uploadResult;
//        }


//        public async Task<DeletionResult> DeletePhotoAsync(string PhotoId)
//        {
//            _cloudinaryService.DeletePhotoAsync(PublicId);
//            var deleteParams = new DeletionParams(PublicId);
//            var Result = await _cloudinary.DestroyAsync(deleteParams);
//            return Result;
//        }
//    }
//}
