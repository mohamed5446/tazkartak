using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.DTO.TripDTOs;

namespace Tazkartk.Application.Interfaces
{
    public interface ITripService
    {
        Task<IReadOnlyList<TripDtos>> GetTripsAsync();
        Task<IReadOnlyList<TripDtos>> GetAvailableTripsAsync();
        Task<IReadOnlyList<TripDtos>> GetCompanyTripsAsync(int companyId);
        Task<IReadOnlyList<TripDtos>> SearchAsync(string? from, string? to, DateOnly? date);
        Task<TripDTO?> GetTripByIdAsync(int id);
        Task<IEnumerable<PassengerDetailsDTO>> GetPassengersAsync(int TripId);
        Task<IReadOnlyList<TicketDTO>?> GetBookingsByTripAsync(int TripId);
        Task<ApiResponse<TripDtos>> AddTripAsync(int companyId, CreateTripDtos DTO);
        Task<ApiResponse<TripDtos>> EditTripAsync(int Id, UpdateTripDtos DTO);
        Task<ApiResponse<string>> DeleteTripAsync(int Id);

        Task<bool> SendReminderEmailAsync(int TripId);
        Task<bool> MarkTripUnavailableAsync(int TripId);
        Task<bool> send_Email_to_passengersAsync(int TripId, EmailDTO DTO);
      //  void DeleteExistingJobs(int tripId);
       // Task<bool> TransferFunds(int TripId);
        Task<List<CreateTripDtos>> ImportFromExcelAsync(int CompanyId, IFormFile file);
        Task deletejobs(int tripId);
        void deletejob(string jobId);
    }
}
