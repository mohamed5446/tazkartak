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


    }
}
