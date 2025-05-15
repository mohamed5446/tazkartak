using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tazkartk.Data;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.Extensions;
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
        private readonly IMapper _mapper;
        public CompanyService(ApplicationDbContext context, IPhotoService photoService, UserManager<Account> accountManager, IConfiguration conf, IMapper mapper)
        {
            _context = context;
            _photoService = photoService;
            _AccountManager = accountManager;
            _conf = conf;
            _mapper = mapper;
        }
        public async Task<ApiResponse<CompanyDTO>> CreateCompanyAsync(CompanyRegisterDTO DTO)
        {
            
            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return ApiResponse<CompanyDTO>.Error("البريد الإلكتروني مستخدم من قبل");
            }
            var Company = _mapper.Map<Company>(DTO);
            Company.Logo = _conf["Logo"];
            Company.UserName = Company.Email;
            Company.EmailConfirmed = true;
            var result = await _AccountManager.CreateAsync(Company, DTO.Password);
            if (!result.Succeeded)
            {
                return ApiResponse<CompanyDTO>.Error(result.Errors.FirstOrDefault()?.Description ?? "حدث خطا ");
            }
            await _AccountManager.AddToRoleAsync(Company, Roles.Company.ToString());
            var Data=_mapper.Map<CompanyDTO>(Company);

            return ApiResponse<CompanyDTO>.success("تمت اضافة الشركة بنجاح ", Data, StatusCode.Created);
        }
            public async Task<IReadOnlyList<CompanyDTO>> GetAllCompaniesAsync()
        {
            return await _context.Companies.AsNoTracking().ProjectTo<CompanyDTO>(_mapper.ConfigurationProvider).ToListAsync();

        }

        public async Task<CompanyDTO?> GetCompanyByIdAsync(int id)
        {
            var Company = await _context.Companies.FindAsync(id);
            return _mapper.Map<CompanyDTO?>(Company);                  
        }
        public async Task<ApiResponse<CompanyDTO>> EditCompanyAsync(int Id, CompanyEditDTO DTO)
        {
            var Company = await _context.Companies.FindAsync(Id);
            if(Company==null)
            {
                return ApiResponse<CompanyDTO>.Error("الشركة غير موجودة ");
            }
             Company.PhoneNumber = DTO.PhoneNumber?.Trim()?? Company.PhoneNumber;
             Company.Name = DTO.Name?.Trim()?? Company.Name;
             Company.City = DTO.City?.Trim() ?? Company.City;
             Company.Street = DTO.Street?.Trim()?? Company.Street;
            if (DTO.Logo != null)
            {
                if (!string.IsNullOrEmpty(Company.Logo))
                {
                    var result = await _photoService.DeletePhotoAsync(Company.Logo);
                    if (result.Error != null)
                    {
                        return ApiResponse<CompanyDTO>.Error($"حدث خطا اثناء تعديل الصورة:{result.Error.Message}",StatusCode.InternalServerError);
                    }
                }
                    var photoResult = await _photoService.AddPhotoAsync(DTO.Logo);
                       if(photoResult.Error!=null)
                       {
                         return ApiResponse<CompanyDTO>.Error($"حدث خطا اثناء تعديل الصورة:{photoResult.Error.Message}",StatusCode.InternalServerError);

                        };
                Company.Logo = photoResult.Url.ToString();
            }
                await _context.SaveChangesAsync();
            var Data = _mapper.Map<CompanyDTO>(Company);

            return ApiResponse<CompanyDTO>.success("تم التعديل بنجاح", Data);
        }

        public async Task<ApiResponse<CompanyDTO>> DeleteCompanyAsync(int CompanyId)
        {
            var company = await _context.Companies.Include(c => c.Trips).FirstOrDefaultAsync(c => c.Id == CompanyId);
            if(company==null)
            {
                return ApiResponse<CompanyDTO>.Error("الشركة غير موجودة ");
            }
            if (!string.IsNullOrEmpty(company.Logo))
            {
                
                var result =await _photoService.DeletePhotoAsync(company.Logo);
                if (result.Error != null)
                {
                    return ApiResponse<CompanyDTO>.Error($"حدث خطا اثناء تعديل الصورة:{result.Error.Message}");
                }
            }
            var has_trips = company.Trips.Any();
            if(has_trips)
            {
                return ApiResponse<CompanyDTO>.Error(" يجب أولاً حذف الرحلات الخاصة بهذه الشركة قبل أن تتمكن من حذفها ");
            }          
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            return ApiResponse<CompanyDTO>.success("تم حذف الشركة بنجاح ");   
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
