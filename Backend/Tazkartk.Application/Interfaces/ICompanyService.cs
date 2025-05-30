using Tazkartk.Application.DTO.CompanyDTOs;
using Tazkartk.Application.DTO.Response;

namespace Tazkartk.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<ApiResponse<CompanyDTO>> CreateCompanyAsync(CompanyRegisterDTO DTO);
        Task<IReadOnlyList<CompanyDTO>> GetAllCompaniesAsync();
        Task<CompanyDTO?> GetCompanyByIdAsync(int id);
        Task<ApiResponse<CompanyDTO>> EditCompanyAsync(int Id, CompanyEditDTO DTO);
        Task<ApiResponse<CompanyDTO>> DeleteCompanyAsync(int CompanyId);
        //Task changeLogos();
    }
}
