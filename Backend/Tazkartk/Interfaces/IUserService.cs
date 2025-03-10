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
        Task<List<UserDetails>> GetUsers();
        Task<ApiResponse<UserDetails?>> AddUser(RegisterDTO DTO, Roles role);
        Task<UserDetails?> GetUserById(int id);
        Task<ApiResponse<UserDetails?>> EditUser(int Id, EditUserDTO DTO);
        Task<ApiResponse<string?>> DeleteUser(int Id);
        
    }
}
