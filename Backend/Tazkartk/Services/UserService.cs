using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Tazkartk.Data;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.UserDTOs;
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
        private readonly IConfiguration _conf;
        private readonly IBookingService _bookingService;
        const string Pattern = "^[^@]+@[^@]+\\.[^@]+$";


        public UserService(ApplicationDbContext context, IPhotoService photoService, UserManager<Account> accountManager, IConfiguration conf, IBookingService bookingService)
        {
            _context = context;
            _photoService = photoService;
            _AccountManager = accountManager;
            _conf = conf;
            _bookingService = bookingService;
        }
        public async Task<List<UserDetails>> GetUsers()
        {
            return await _context.Users.AsNoTracking()
                .Select(u => new UserDetails
                {
                    Email = u.Email,
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhotoUrl = u.photo,
                    phoneNumber = u.PhoneNumber,
                }).ToListAsync();
        }

        public async Task<UserDetails?> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            
            if (user == null)
            {
                return null;
            }
            return new UserDetails
                {
                    Id = id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    phoneNumber = user.PhoneNumber,
                    PhotoUrl = user.photo   
                };
        }
        public async Task<ApiResponse<UserDetails?>> AddUser(RegisterDTO DTO, Roles role)
        {

            bool match = Regex.IsMatch(DTO.Email, Pattern);
            if (!match)
            {
                return new ApiResponse<UserDetails?>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "البريد الإلكتروني غير صالح"
                };
            }
            var response = new ApiResponse<UserDetails?>();
            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return new ApiResponse<UserDetails?>()
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "البريد الإلكتروني مستخدم من قبل "
                };
            }
            var user = new User
            {
                FirstName = DTO.FirstName.Trim(),
                LastName = DTO.LastName.Trim(),
                Email = DTO.Email.Trim(),
                PhoneNumber = DTO.PhoneNumber.Trim(),
                UserName = DTO.Email.Trim(),
                photo = _conf["Avatar"],
                EmailConfirmed = true
            };
            var result = await _AccountManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                return new ApiResponse<UserDetails?>()
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = result.Errors.FirstOrDefault()?.Description ?? "حدث خطا "
                };
            }
            await _AccountManager.AddToRoleAsync(user, role.ToString());
            return new ApiResponse<UserDetails?>()
            {
                Success = true,
                StatusCode=StatusCode.Created,
                message = "تم اضافة مستخدم بنجاح",
                Data = new UserDetails
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    phoneNumber = user.PhoneNumber,
                    PhotoUrl = user.photo
                }
            };

        }
        public async Task<ApiResponse<UserDetails?>> EditUser(int Id, EditUserDTO DTO)
        {
            var user = await _context.Users.FindAsync(Id);
            if(user==null)
            {
                return new ApiResponse<UserDetails?>
                {
                    Success = false,
                    StatusCode = StatusCode.NotFound,
                    message = "المستخدم غير موجود "
                };
            }
            if (!string.IsNullOrEmpty(DTO.firstName))
            {
                user.FirstName = DTO.firstName.Trim();
            }

            if (!string.IsNullOrEmpty(DTO.lastName))
            {
                user.LastName = DTO.lastName.Trim();
            }

            if (!string.IsNullOrEmpty(DTO.Phone))
            {
                user.PhoneNumber = DTO.Phone.Trim();
            }


            if (DTO.photo != null)
            {
                if (!string.IsNullOrEmpty(user.photo))
                {
                    var res = await _photoService.DeletePhotoAsync(user.photo);
                    if (res.Error != null)
                    {
                        return new ApiResponse<UserDetails?>
                        {
                            Success = false,
                            StatusCode = StatusCode.BadRequest,
                            message = res.Error.Message
                        };
                    }
                }

                var photoResult = await _photoService.AddPhotoAsync(DTO.photo);
                if (photoResult.Error != null)
                {
                    return new ApiResponse<UserDetails?>
                    {
                        Success = false,
                        StatusCode = StatusCode.BadRequest,
                        message = photoResult.Error.Message
                    };
                }
                user.photo = photoResult.Url.ToString();
            }
            await _context.SaveChangesAsync();
            return new ApiResponse<UserDetails?>()
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                message = "تم التعديل بنجاح ",
                Data = new UserDetails
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    phoneNumber = user.PhoneNumber,
                    PhotoUrl = user.photo
                }
            };
        }
        
        public async Task<ApiResponse<string?>> DeleteUser(int Id)
        {
            var user = await _context.Users.Include(u=>u.books).FirstOrDefaultAsync(u => u.Id == Id);
            if(user==null)
            {
                return new ApiResponse<string?>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "user not found",
                };
            }
            if(user.books!=null && user.books.Any(b=>!b.IsCanceled))
            {
                return new ApiResponse<string?>
                {
                    Success=false,
                    StatusCode=StatusCode.BadRequest,
                    message= "لا يمكن حذف المستخدم لأن لديه حجوزات حالية"
                };
            }
            var tickets = user.books.Where(b => b.IsCanceled);
            if (tickets!=null)  
            {
                foreach (var ticket in tickets)
                {
                    if (ticket.IsCanceled)
                    {
                        _bookingService.DeleteBooking(ticket.BookingId);
                    }
                }
            }
              if (!string.IsNullOrEmpty(user.photo))
                {
                   var res= await _photoService.DeletePhotoAsync(user.photo);
                   if(res.Error != null)
                    {
                    return new ApiResponse<string?>
                    {
                        Success = false,
                        StatusCode = StatusCode.BadRequest,
                        message =res.Error.Message
                    };
                }

                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return new ApiResponse<string?>()
                {
                    StatusCode=StatusCode.Ok,
                    Success = true,
                    message = "تم حذف المستخدم بنجاح "
                };

            }
            
           

        }
       
    }

