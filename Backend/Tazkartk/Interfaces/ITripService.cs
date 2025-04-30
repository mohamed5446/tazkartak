using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.TripDTOs;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Models.Enums;

namespace Tazkartk.Interfaces
{
    public interface ITripService
    {
        Task<IEnumerable<TripDtos>> GetTripsAsync();
        Task<IEnumerable<TripDtos>> GetAvailableTripsAsync();
        Task<IEnumerable<TripDtos>> GetCompanyTripsAsync(int companyId);
        Task<IEnumerable<TripDtos>> SearchAsync(string? from, string? to, DateOnly? date);
        Task<TripDTO?> GetTripByIdAsync(int id);
        Task<IEnumerable<PassengerDetailsDTO>> GetPassengersAsync(int TripId);
        Task<List<TicketDTO>?> GetBookingsByTripAsync(int TripId);
        Task<ApiResponse<TripDtos>> AddTripAsync(int companyId,CreateTripDtos DTO);
        Task<ApiResponse<TripDtos>> EditTripAsync(int Id, UpdateTripDtos DTO);
        Task<ApiResponse<string>> DeleteTripAsync(int Id);
        Task<bool>SendReminderEmailAsync(int TripId);
        Task<bool> MarkTripUnavailableAsync(int TripId);
        Task<bool> send_Email_to_passengersAsync(int TripId, EmailDTO DTO);
        void DeleteExistingJobs(int tripId);
        Task<bool> transfer(int TripId);

    }
}
