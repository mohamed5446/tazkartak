using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.TripDTOs;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Models.Enums;

namespace Tazkartk.Interfaces
{
    public interface ITripService
    {
        Task<IEnumerable<TripDtos>> GetTrips();
        Task<ApiResponse<TripDtos?>> AddTrip(int companyId,CreateTripDtos DTO);
        Task<TripDTO?> GetTripById(int id);
        Task<ApiResponse<TripDtos?>> EditTrip(int Id, UpdateTripDtos DTO);
        Task<ApiResponse<string?>> DeleteTrip(int Id);
        Task<IEnumerable<TripDtos>> GetCompanyTrips(int companyId);
        Task<IEnumerable<TripDtos>> Search(string? from, string? to, DateOnly? date);
        Task<IEnumerable<UserDetails>> GetPassengers(int TripId);
        Task<bool>SendReminderEmail(int TripId);

    }
}
