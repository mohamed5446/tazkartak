using Microsoft.EntityFrameworkCore;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDetails>> GetUsers();
        Task<UserDetails?> AddUser(RegisterDTO DTO, Roles role);
        Task<UserDetails> GetUserDetailsById(int id);
        Task<User> GetUserById(int id);
        Task<UserDetails?> EditUser(User user, EditUserDTO DTO);
        Task DeleteUser(User user);
        
    }
}
