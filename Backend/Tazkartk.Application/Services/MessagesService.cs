using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.Repository;
using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessagesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<string>> SendMessage(MessageDTO DTO)
        {
            Message message = new Message
            {
                Subject = DTO.Subject,
                Body = DTO.Body,
                Email = DTO.Email,
                Name = DTO.Name,
            };
           await _unitOfWork.Messages.Add(message);
            await _unitOfWork.CompleteAsync();
            return ApiResponse<string>.success("تم ارسال الرسالة بنجاح سيتم الرد في اقرب وقت");
        }
        public async Task<List<MessageDTO>>GetAllMessages()
        {
            return await _unitOfWork.Messages.ProjectToList<MessageDTO>();
        }

    }
}
