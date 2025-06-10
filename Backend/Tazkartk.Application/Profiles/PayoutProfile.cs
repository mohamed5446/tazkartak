using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.DTO;
using Tazkartk.Application.Extensions;
using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Profiles
{
    public class PayoutProfile:Profile
    {
        public PayoutProfile()
        {
            CreateMap<Payout, payoutDTO>()
                .ForMember(dest => dest.PayoutId, opt => opt.MapFrom(src => src.PayoutId))
                .ForMember(dest => dest.amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.WalletNumber, opt => opt.MapFrom(src => src.WalletNumber))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToEgyptDateString()));
        }
    }
}
