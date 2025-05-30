
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.TripDTOs;
using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Repository
{
    public interface ITripRepository:IGenericRepository<Trip>
    {
        Task<IEnumerable<PassengerDetailsDTO>> GetPassengersAsync(int TripId);
        Task<Trip?> GetTripWithBookingAndUer(int TripId);
        Task<List<Trip>> AddRange(List<CreateTripDtos> trips,int CompanyId);
    }
}
