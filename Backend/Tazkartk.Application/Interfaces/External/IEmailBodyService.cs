using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.DTO;

namespace Tazkartk.Application.Interfaces.External
{
    public interface IEmailBodyService
    {
        string ConfirmationEmailBody(string OTP);
        string ResetPasswordEmailBody(string OTP);
        string TripReminderEmailBody(TicketDTO ticket);
        string BalanceTransferEmailBody(string CompanyName, int TripId, string From, string To, string Date, double amount);
        string PayoutConfirmationEmailBody(string companyName, double amount, string date, string walletNumber);
    }
}
