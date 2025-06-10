using Tazkartk.Application;
using Tazkartk.Application.DTO.AccontDTOs;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.DTO.UserDTOs;

namespace Tazkartk.Application.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserDetails>> GetUsersAsync(Roles Role);
        Task<UserDetails?> GetUserByIdAsync(int id);
        Task<ApiResponse<UserDetails>> AddUserAsync(RegisterDTO DTO, Roles role);
        Task<ApiResponse<UserDetails>> EditUserAsync(int Id, EditUserDTO DTO);
        Task<ApiResponse<string>> DeleteUserAsync(int Id);



    }
}
