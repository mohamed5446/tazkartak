using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Tazkartk.Data;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.Interfaces;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoService _photoService;
        private readonly UserManager<Account> _AccountManager;
        private readonly IConfiguration _conf;
        const string Pattern= "^[^@]+@[^@]+\\.[^@]+$";
        public CompanyService(ApplicationDbContext context, IPhotoService photoService, UserManager<Account> accountManager, IConfiguration conf)
        {
            _context = context;
            _photoService = photoService;
            _AccountManager = accountManager;
            _conf = conf;
        }
        public async Task<ApiResponse<CompanyDTO?>> CreateCompany(CompanyRegisterDTO DTO)
        {
            
            bool match = Regex.IsMatch(DTO.Email, Pattern );
            if(!match )
            {
                return new ApiResponse<CompanyDTO?>
                {
                    Success=false,
                    StatusCode=StatusCode.BadRequest,
                    message= "البريد الإلكتروني غير صالح"
                };
            }

            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return new ApiResponse<CompanyDTO?>()
                {
                    StatusCode=StatusCode.BadRequest,
                    Success = false,
                    message = "البريد الإلكتروني مستخدم من قبل",                  
                };
            }
            var Company = new Company
            {
                Name = DTO.Name,
                Email = DTO.Email.Trim(),
                PhoneNumber = DTO.Phone,
                UserName = DTO.Email.Trim(),
                City = DTO.city,
                Street = DTO.street,
                EmailConfirmed=true,
                Logo = _conf["Logo"]
                
            };
            var result = await _AccountManager.CreateAsync(Company, DTO.Password);
            if (!result.Succeeded)
            {
                return new ApiResponse<CompanyDTO?>()
                {
                    StatusCode=StatusCode.BadRequest,
                    Success = false,
                    message = result.Errors.FirstOrDefault()?.Description ?? "حدث خطا "
                };
            }
            await _AccountManager.AddToRoleAsync(Company, Roles.Company.ToString());
            return new ApiResponse<CompanyDTO?>
            {
                StatusCode=StatusCode.Created,
                Success=true,
                message="تمت اضافة الشركة بنجاح ",
                Data=new CompanyDTO
                {
                    Id = Company.Id,
                    Email = Company.Email,
                    Name = Company.Name,
                    Phone = Company.PhoneNumber,
                    City = Company.City,
                    Street = Company.Street,
                }
            };
           
        }
            public async Task<List<CompanyDTO>> GetAllCompanies()
        {
            return await _context.Companies.AsNoTracking().Select(c => new CompanyDTO
            {
                Id = c.Id,
                Name = c.Name,
                City = c.City,
                Street = c.Street,
                Email = c.Email,
                Logo = c.Logo,
                Phone = c.PhoneNumber,
                Balance=c.Balance
                
            }).ToListAsync();
        }

        public async Task<CompanyDTO?> GetCompanyById(int id)
        {
            var Company = await _context.Companies.FindAsync(id);
            if (Company == null)
            {
                return null;
            }
            return new CompanyDTO
            {
                Id = Company.Id,
                Name = Company.Name,
                Email = Company.Email,
                Phone = Company.PhoneNumber,
                City = Company.City,
                Street = Company.Street,
                Logo = Company.Logo,
                Balance=Company.Balance
            };
            
        }
        public async Task<ApiResponse<CompanyDTO?>> EditCompany(int Id, CompanyEditDTO DTO)
        {
            var Company = await _context.Companies.FindAsync(Id);
            if(Company==null)
            {
                return new ApiResponse<CompanyDTO?>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "الشركة غير موجودة "
                };
            }
            if (!string.IsNullOrEmpty(DTO.PhoneNumber))
            {
                Company.PhoneNumber = DTO.PhoneNumber.Trim();
            }
            if (!string.IsNullOrEmpty(DTO.Name))
            {
                Company.Name = DTO.Name.Trim();
            }
            if (!string.IsNullOrEmpty(DTO.City))
            {
                Company.City = DTO.City.Trim();
            }
            if (!string.IsNullOrEmpty(DTO.Street))
            {
                Company.Street = DTO.Street.Trim();
            }
                if (DTO.Logo != null)
                {
                    if (!string.IsNullOrEmpty(Company.Logo))
                    {
                      var result=  await _photoService.DeletePhotoAsync(Company.Logo);
                      if(result.Error!=null)
                        {
                            return new ApiResponse<CompanyDTO?>
                            {
                                Success = false,
                                message = "error while updating the logo ",
                                StatusCode=StatusCode.BadRequest,
                                
                            };
                        }
                    }

                    var photoResult = await _photoService.AddPhotoAsync(DTO.Logo);
                       if(photoResult.Error!=null)
                        {
                        return new ApiResponse<CompanyDTO?>
                        {
                            Success = false,
                            message = "error while updating the logo ",
                            StatusCode=StatusCode.BadRequest,
                        };
                    }
                    Company.Logo = photoResult.Url.ToString();
                }
                await _context.SaveChangesAsync();
                return new ApiResponse<CompanyDTO?>
                {
                    StatusCode=StatusCode.Ok,
                    Success = true,
                    message = "تم التعديل بنجاح",
                    Data = new CompanyDTO
                    {
                        Id = Company.Id,
                        Name = Company.Name,
                        Email = Company.Email,
                        Phone = Company.PhoneNumber,
                        City = Company.City,
                        Street = Company.Street,
                        Logo = Company.Logo,
                    }
                };  
        }

        public async Task<ApiResponse<CompanyDTO?>> DeleteCompany(int CompanyId)
        {
            var company = await _context.Companies.Include(c => c.Trips).FirstOrDefaultAsync(c => c.Id == CompanyId);
            if(company==null)
            {
                return new ApiResponse<CompanyDTO?>
                {
                    Success = false,
                    message = "الشركة غير موجودة",
                    StatusCode = StatusCode.BadRequest
                };
            }
            if (!string.IsNullOrEmpty(company.Logo))
            {
                
                var result =await _photoService.DeletePhotoAsync(company.Logo);
                if(result.Error != null)
                {
                    return new ApiResponse<CompanyDTO?>
                    {
                        Success = false,
                        message = "Error while deleting Logo",
                        StatusCode=StatusCode.BadRequest,
                    };
                }
            }
            var has_trips = company.Trips.Any();
            if(has_trips)
            {
                return new ApiResponse<CompanyDTO?>
                {
                    Success = false,
                    message = "يجب أولاً حذف الرحلات الخاصة بهذه الشركة قبل أن تتمكن من حذفها ",
                    StatusCode = StatusCode.BadRequest
                   
                };
            }          
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
                return new ApiResponse<CompanyDTO?>
                {
                    Success = true,
                    message = "تم حذف الشركة بنجاح ",
                    StatusCode=StatusCode.Ok
                    
                };   
        }

        public async Task changeLogos()
        {
            var comps = await _context.Companies.ToListAsync();
            foreach(var comp in comps)
            {
                comp.Logo = _conf["Logo"];
            }
            await _context.SaveChangesAsync();  
        }
       



    }
}
