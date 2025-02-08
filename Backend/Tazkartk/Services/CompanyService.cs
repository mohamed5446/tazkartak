using Microsoft.EntityFrameworkCore;
using Tazkartk.Data;
using Tazkartk.DTO;
using Tazkartk.Interfaces;
using Tazkartk.Models;

namespace Tazkartk.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPhotoService _photoService;

        public CompanyService(ApplicationDbContext context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
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
            if (company == null)
            {
                return null;
            }
            return company; 
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
            _context.SaveChanges();
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
            _context.SaveChanges();

        }






    }
}
