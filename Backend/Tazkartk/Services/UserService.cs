using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using System.Text.RegularExpressions;
using Tazkartk.Data;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Extensions;
using Tazkartk.Interfaces;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoService _photoService;
        private readonly UserManager<Account> _AccountManager;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _conf;

        public UserService(ApplicationDbContext context, IPhotoService photoService, UserManager<Account> accountManager, IConfiguration conf, IBookingService bookingService, IMapper mapper)
        {
            _context = context;
            _photoService = photoService;
            _AccountManager = accountManager;
            _conf = conf;
            _bookingService = bookingService;
            _mapper = mapper;
        }
        public async Task<IReadOnlyList<UserDetails>> GetUsersAsync(Roles Role)
        {
            var users = await _AccountManager.GetUsersInRoleAsync(Role.ToString());
            return _mapper.Map<IReadOnlyList<UserDetails>>(users);
        }
        public async Task<UserDetails?> GetUserByIdAsync(int id)
        {
            return await _context.Users.ProjectTo<UserDetails>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(u=>u.Id== id);
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
            user.UserName=user.Email;
            var result = await _AccountManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                return ApiResponse<UserDetails>.Error(result.Errors.FirstOrDefault()?.Description ?? "حدث خطا ");
            }
            await _AccountManager.AddToRoleAsync(user, role.ToString());
           var data=_mapper.Map<UserDetails>(user);
            return ApiResponse<UserDetails>.success("تم اضافة مستخدم بنجاح", data, StatusCode.Created);
        }
        public async Task<ApiResponse<UserDetails>> EditUserAsync(int Id, EditUserDTO DTO)
        {
            var user = await _context.Users.FindAsync(Id);
            if(user==null)
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
                    var res = await _photoService.DeletePhotoAsync(user.photo);
                    if (res.Error != null)
                    {
                        return ApiResponse<UserDetails>.Error($"حدث خطا اثناء تعديل الصورة:{res.Error.Message}",StatusCode.InternalServerError);
                    }
                }
                var photoResult = await _photoService.AddPhotoAsync(DTO.photo);
                if (photoResult.Error != null)
                {
                    return ApiResponse<UserDetails>.Error($"حدث خطا اثناء تعديل الصورة:{photoResult.Error.Message}",StatusCode.InternalServerError);
                }
                user.photo = photoResult.Url.ToString();
            }
            await _context.SaveChangesAsync();
            var data=_mapper.Map<UserDetails>(user);
            return ApiResponse<UserDetails>.success("تم التعديل بنجاح ",data);
        }
        public async Task<ApiResponse<string>> DeleteUserAsync(int Id)
        {
            var user = await _context.Users.Include(u=>u.books).ThenInclude(v=>v.trip).FirstOrDefaultAsync(u => u.Id == Id);
            if(user==null)
            {
                return ApiResponse<string>.Error("المستخدم غير موجود");
            }
            if(user.books!=null && (user.books.Any(b=>!b.IsCanceled||b.trip.Avaliblility)))
            {
                return ApiResponse<string>.Error("لا يمكن حذف المستخدم لأن لديه حجوزات حالية");
            }
            var tickets = user.books.Where(b => b.IsCanceled);
            if (tickets!=null)  
            {
                foreach (var ticket in tickets)
                {
                    if (ticket.IsCanceled)
                    {
                      await _bookingService.DeleteBookingAsync(ticket.BookingId);
                    }
                }
            }
              if (!string.IsNullOrEmpty(user.photo))
                {
                   var res= await _photoService.DeletePhotoAsync(user.photo);
               
                    if (res.Error != null)
                    {
                        return ApiResponse<string>.Error($"حدث خطا اثناء تعديل الصورة:{res.Error.Message}",StatusCode.InternalServerError);
                    }
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            return ApiResponse<string>.success("تم حذف المستخدم بنجاح");

            }
        }
       
    }

