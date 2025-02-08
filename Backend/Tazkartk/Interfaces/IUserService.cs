using Tazkartk.DTO;
using Tazkartk.Models;

namespace Tazkartk.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDetails>> GetUsers();
        Task<UserDetails> GetUserDetailsById(int id);
        Task<User> GetUserById(int id);
        Task<UserDetails?> EditUser(User user, UserDTO DTO);
        Task DeleteUser(User user);
    }
}
