using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Response;

namespace Tazkartk.Application.Interfaces
{
    public interface IMessagesService
    {
        Task<ApiResponse<string>> SendMessage(MessageDTO DTO);
        Task<List<MessageDTO>> GetAllMessages();


    }
}
