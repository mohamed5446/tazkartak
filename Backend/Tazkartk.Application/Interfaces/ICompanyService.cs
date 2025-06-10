using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.CompanyDTOs;
using Tazkartk.Application.DTO.Payments;
using Tazkartk.Application.DTO.Response;

namespace Tazkartk.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<IReadOnlyList<CompanyDTO>> GetAllCompaniesAsync();
        Task<CompanyDTO?> GetCompanyByIdAsync(int id);
        Task<ApiResponse<CompanyDTO>> CreateCompanyAsync(CompanyRegisterDTO DTO);
        Task<ApiResponse<CompanyDTO>> EditCompanyAsync(int Id, CompanyEditDTO DTO);
        Task<ApiResponse<CompanyDTO>> DeleteCompanyAsync(int CompanyId);
       Task<ApiResponse<string>> WithdrawlBalance(int CompanyId,WithdrawDTO DTO);
        Task<List<payoutDTO>> GetCompanyPayouts(int CompanyId);
        Task<ApiResponse<string>> IncreaseCompanyBalance(int CompanyId);
    }
}
