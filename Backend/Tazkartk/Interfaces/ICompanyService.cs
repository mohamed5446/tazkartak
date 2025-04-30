using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.Models;

namespace Tazkartk.Interfaces
{
    public interface ICompanyService
    {
        Task <ApiResponse<CompanyDTO>> CreateCompanyAsync(CompanyRegisterDTO DTO);
        Task<List<CompanyDTO>> GetAllCompaniesAsync();
      //  Task<CompanyDTO> GetCompanyDetailsById(int id);
        Task<CompanyDTO?> GetCompanyByIdAsync(int id);
        Task<ApiResponse<CompanyDTO>> EditCompanyAsync(int Id, CompanyEditDTO DTO);
        Task <ApiResponse<CompanyDTO>>DeleteCompanyAsync(int CompanyId);
        Task changeLogos();
    }
}
