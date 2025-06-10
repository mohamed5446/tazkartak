using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Payments;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.DTO.Ticket;

namespace Tazkartk.Application.Interfaces.External
{
    public interface IPaymentGateway
    {
        Task<string> CreatePaymentIntentAsync( UserBookingDetails DTO,double amount);
        Task<bool> RefundTransactionAsync(string transactionId, double amountCents);
        Task<WebhookRequest> HandleCallBack(HttpRequest request);
        Task<dispurseresponse> DispurseAsync(string issuer, string walletnumber, double amount);
        Task<double> BalanceInquiry();
        
    }
}
