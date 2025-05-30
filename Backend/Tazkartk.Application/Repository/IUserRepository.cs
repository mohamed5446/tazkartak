using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Repository
{
    public interface IUserRepository:IGenericRepository<User>
    {
        Task<User?> GetUserWithBookingandTrip(int id);
        Task<User> GetUserWtihBookingDetails(int userId);
  
    }
}
