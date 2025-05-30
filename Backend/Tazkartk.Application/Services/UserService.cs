using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Tazkartk.Application.DTO.AccontDTOs;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.DTO.UserDTOs;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models;


namespace Tazkartk.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<Account> _AccountManager;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _conf;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;
        public UserService( UserManager<Account> accountManager, IConfiguration conf, IBookingService bookingService, IMapper mapper, IUnitOfWork unitOfWork, IPhotoService photoService)
        {
      
            _AccountManager = accountManager;
            _conf = conf;
            _bookingService = bookingService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
        }
        public async Task<IReadOnlyList<UserDetails>> GetUsersAsync(Roles Role)
        {
            var users = await _AccountManager.GetUsersInRoleAsync(Role.ToString());
            return _mapper.Map<List<UserDetails>>(users);
        }
        public async Task<UserDetails?> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.Users.GetById<UserDetails>(u => u.Id == id);
        }
        public async Task<ApiResponse<UserDetails>> AddUserAsync(RegisterDTO DTO, Roles role)
        {
            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return ApiResponse<UserDetails>.Error("البريد الإلكتروني مستخدم من قبل");
            }
            var user = _mapper.Map<User>(DTO);
            user.photo = _conf["Avatar"];
            user.EmailConfirmed = true;
            user.UserName = user.Email;
            var result = await _AccountManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                return ApiResponse<UserDetails>.Error(result.Errors.FirstOrDefault()?.Description ?? "حدث خطا ");
            }
            await _AccountManager.AddToRoleAsync(user, role.ToString());
            var data = _mapper.Map<UserDetails>(user);
            return ApiResponse<UserDetails>.success("تم اضافة مستخدم بنجاح", data, StatusCode.Created);
        }
        public async Task<ApiResponse<UserDetails>> EditUserAsync(int Id, EditUserDTO DTO)
        {
            var user = await _unitOfWork.Users.GetById(Id);
            if (user == null)
            {
                return ApiResponse<UserDetails>.Error("المستخدم غير موجود", StatusCode.NotFound);
            }

            user.FirstName = DTO.firstName?.Trim() ?? user.FirstName;
            user.LastName = DTO.lastName?.Trim() ?? user.LastName;
            user.PhoneNumber = DTO.Phone?.Trim() ?? user.PhoneNumber;

            if (DTO.photo != null)
            {
                if (!string.IsNullOrEmpty(user.photo))
                {
                await _photoService.DeletePhotoAsync(user.photo);                
                }
                var photoResult = await _photoService.AddPhotoAsync(DTO.photo);
                user.photo = photoResult;
            }
            await _unitOfWork.CompleteAsync();
            var data = _mapper.Map<UserDetails>(user);
            return ApiResponse<UserDetails>.success("تم التعديل بنجاح ", data);
        }
        public async Task<ApiResponse<string>> DeleteUserAsync(int Id)
        {
            var user = await _unitOfWork.Users.GetUserWithBookingandTrip(Id);
            if (user == null)
            {
                return ApiResponse<string>.Error("المستخدم غير موجود");
            }
            if (user.books != null && user.books.Any(b => !b.IsCanceled || b.trip.Avaliblility))
            {
                return ApiResponse<string>.Error("لا يمكن حذف المستخدم لأن لديه حجوزات حالية");
            }
            var tickets = user.books.Where(b => b.IsCanceled);
            if (tickets != null)
            {
                foreach (var ticket in tickets)
                {
                  await _bookingService.DeleteBookingAsync(ticket.BookingId);
                }
            }
            if (!string.IsNullOrEmpty(user.photo))
            {
                await _photoService.DeletePhotoAsync(user.photo);
            }
            _unitOfWork.Users.Remove(user);
            await _unitOfWork.CompleteAsync();  
            return ApiResponse<string>.success("تم حذف المستخدم بنجاح");

        }
    }

}

