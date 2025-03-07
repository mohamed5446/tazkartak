using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tazkartk.Data;
using Tazkartk.DTO.CompanyDTOs;
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

        public CompanyService(ApplicationDbContext context, IPhotoService photoService, UserManager<Account> accountManager)
        {
            _context = context;
            _photoService = photoService;
            _AccountManager = accountManager;
        }
        public async Task<CompanyDTO?> CreateCompany(CompanyRegisterDTO DTO)
        {

            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return null;
            }
            var Company = new Company
            {
                Name = DTO.Name,
                Email = DTO.Email,
                PhoneNumber = DTO.Phone,
                UserName = DTO.Email,
                City = DTO.city,
                Street = DTO.street,
                EmailConfirmed=true
            };
            var result = await _AccountManager.CreateAsync(Company, DTO.Password);
            if (!result.Succeeded)
            {
                return null;
            }
            await _AccountManager.AddToRoleAsync(Company, Roles.Company.ToString());
            return new CompanyDTO
            {
                Id = Company.Id,
                Email = Company.Email,
                Name = Company.Name,
                Phone=Company.PhoneNumber,
                City = Company.City,
                Street= Company.Street,
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
                Phone = c.PhoneNumber
            }).ToListAsync();
        }

        public async Task<Company?> GetCompanyById(int id)
        {      
          var company=await  _context.Companies.FindAsync(id);
            return company != null ? company : null;
        }

        public async Task<CompanyDTO?> GetCompanyDetailsById(int id)
        {
            var Company = await _context.Companies.FindAsync(id);
            if (Company == null) return null;
            return new CompanyDTO
            {
                Id = Company.Id,
                Name = Company.Name,
                Email = Company.Email,
                Phone = Company.PhoneNumber,
                City = Company.City,
                Street = Company.Street,
                Logo = Company.Logo,
            };
        }
        public async Task<CompanyDTO?> EditCompany(Company Company, CompanyEditDTO DTO)
        {
            if (!string.IsNullOrEmpty(DTO.PhoneNumber))
            {
                Company.PhoneNumber = DTO.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(DTO.Name))
            {
                Company.Name = DTO.Name;
            }
            if (!string.IsNullOrEmpty(DTO.City))
            {
                Company.City = DTO.City;
            }
            if (!string.IsNullOrEmpty(DTO.Street))
            {
                Company.Street = DTO.Street;
            }
            if (DTO.Logo != null)
            {
                if (!string.IsNullOrEmpty(Company.Logo))
                {
                    await _photoService.DeletePhotoAsync(Company.Logo);
                }

                var photoResult = await _photoService.AddPhotoAsync(DTO.Logo);
                Company.Logo = photoResult.Url.ToString();
            }
            await _context.SaveChangesAsync();
            return new CompanyDTO
            {
                Id = Company.Id,
                Name = Company.Name,
                Email = Company.Email,
                Phone = Company.PhoneNumber,
                City = Company.City,
                Street = Company.Street,
                Logo = Company.Logo,
            };
        }

        public async Task DeleteCompany(Company company)
        {
            if (!string.IsNullOrEmpty(company.Logo))
            {
                await _photoService.DeletePhotoAsync(company.Logo);
            }
            _context.Companies.Remove(company);
           await  _context.SaveChangesAsync();

        }






    }
}
