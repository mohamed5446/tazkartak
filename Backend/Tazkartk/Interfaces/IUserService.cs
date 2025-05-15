using Microsoft.EntityFrameworkCore;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserDetails>> GetUsersAsync(Roles Role);
       // Task<IEnumerable<UserDetails>> GetAdmins();
        Task<ApiResponse<UserDetails>> AddUserAsync(RegisterDTO DTO, Roles role);
        Task<UserDetails?> GetUserByIdAsync(int id);
        Task<ApiResponse<UserDetails>> EditUserAsync(int Id, EditUserDTO DTO);
        Task<ApiResponse<string>> DeleteUserAsync(int Id);
        


    }
}
