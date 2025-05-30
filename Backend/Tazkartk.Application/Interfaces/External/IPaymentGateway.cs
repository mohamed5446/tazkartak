using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Response;

namespace Tazkartk.Application.Interfaces
{
    public interface IPaymentGateway
    {
        Task<string> CreatePaymentIntentAsync(int userId, int tripId, List<int> SeatNumbers, double amount, string firstname, string lastname, string PhoneNumber, string Email);

        // Task<string> CreatePaymentIntentAsync(BookingDTO DTO, double amount, UserDetailsDTO UserDTO);
        Task<bool> RefundTransactionAsync(string transactionId, double amountCents);
        Task<WebhookRequest> HandleCallBack(HttpRequest request);
        //public bool ValidateHmac(paymobresponse callback, string hmac);
        //Task<ApiResponse<bool>> handleCallback(HttpRequest request);
    }
}
