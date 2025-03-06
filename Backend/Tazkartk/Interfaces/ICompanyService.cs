using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.Models;

namespace Tazkartk.Interfaces
{
    public interface ICompanyService
    {
        Task<List<CompanyDTO>> GetAllCompanies();
        Task<CompanyDTO> GetCompanyDetailsById(int id);
        Task<Company?> GetCompanyById(int id);
        Task<CompanyDTO?> EditCompany(Company company, CompanyEditDTO DTO);
        Task DeleteCompany(Company user);
    }
}
