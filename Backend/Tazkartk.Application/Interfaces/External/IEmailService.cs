using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Email;
namespace Tazkartk.Application.Interfaces.External
{
    public interface IEmailService
    {
        Task SendEmail(EmailRequest emailRequest);
    }
}
