
using Tazkartk.Application.DTO;
using AutoMapper;
using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Profiles
{
    public class MessageProfile:Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageDTO>()
                .ForMember(dest=>dest.MessageId,opt=>opt.MapFrom(src=>src.Id));

        }
    }
}
