using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.Models;

namespace Tazkartk.Interfaces
{
    public interface ICompanyService
    {
        Task <ApiResponse<CompanyDTO>> CreateCompany(CompanyRegisterDTO DTO);
        Task<List<CompanyDTO>> GetAllCompanies();
      //  Task<CompanyDTO> GetCompanyDetailsById(int id);
        Task<CompanyDTO?> GetCompanyById(int id);
        Task<ApiResponse<CompanyDTO?>> EditCompany(int Id, CompanyEditDTO DTO);
        Task <ApiResponse<CompanyDTO>>DeleteCompany(int CompanyId);
        Task changeLogos();
    }
}
