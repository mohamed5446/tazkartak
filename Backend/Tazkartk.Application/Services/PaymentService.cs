using Microsoft.AspNetCore.Http;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Payments;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.Interfaces.External;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models.Enums;

namespace Tazkartk.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IPaymentGateway _paymentGateway;
        private readonly IBookingService _bookingService;

        public PaymentService( IUnitOfWork unitOfWork, IPaymentGateway paymentGateway, IBookingService bookingService)
        {

            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _bookingService = bookingService;
        }
        public async Task<bool> TransferFunds(int TripId)
        {
            var trip = await _unitOfWork.Trips.GetById(t => t.TripId == TripId, t => t.company, t => t.seats);
            if (trip == null) throw new Exception("error happened");
            var count = trip.seats.Where(s => s.State == SeatState.Booked).Count();
            var total = count * trip.Price;
            var company = trip.company;
            company.Balance += total;
            _unitOfWork.Companies.Update(company);
            _unitOfWork.CompleteAsync();
            return true;
        }
        public async Task<IReadOnlyList<PaymentDTO>> GetAllPaymentsAsync()
        {
            return await _unitOfWork.Payments.ProjectToList<PaymentDTO>();
        }

        public Task<PaymentDTO> GetPaymentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ApiResponse<bool>> handleCallback(HttpRequest Request)
        {
            var result = await _paymentGateway.HandleCallBack(Request);
            if (!result.IsSuccessful)
            {
                return ApiResponse<bool>.Error("error");
            }
        
                if (result.IsRefunded)
                {
                    var Done = await _bookingService.CancelAsync(result.TransactionId);
                    if (!Done)
                    {
                        return ApiResponse<bool>.Error("failed to refund");
                    }
                    return ApiResponse<bool>.success("refunded successfully");
                }
                else
                {
                    var done = await _bookingService.ConfirmBookingAsync(result.extra, result.TransactionId, result.PaymentMethod);
                    if (!done)
                    {
                        return ApiResponse<bool>.Error("failed to confirm");
                    }
                    return ApiResponse<bool>.success("confirmed successfully");
                }
            }
        //public async Task<string> Genrateaccesstoken()
        //{
        //    return await _paymentGateway.GenerateAccessTokenn();
        //}

        public async Task<dispurseresponse> DispurseAsync(string issuer,string walletnumber,double amount)
        {
            var balance = await _paymentGateway.BalanceInquiry();
            if (amount > balance)
            {
                return new dispurseresponse
                {
                    Success = false,
                    message = "برجاء التواصل معنا ",
                };
                }
            return await _paymentGateway.DispurseAsync(issuer,walletnumber,amount);
        }
       public async Task<double> BalanceInquiry()
        {
            var res = await _paymentGateway.BalanceInquiry();
            return res;
        }

    }
        }
 
