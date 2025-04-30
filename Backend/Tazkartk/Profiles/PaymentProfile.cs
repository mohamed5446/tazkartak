using AutoMapper;
using Tazkartk.DTO;
using Tazkartk.Models;
using System.Globalization;


namespace Tazkartk.Profiles
{
    public class PaymentProfile:Profile
    {
        public PaymentProfile()
        {
            var arabicCulture = new CultureInfo("ar-EG");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            arabicCulture.DateTimeFormat.AMDesignator = "صباحا";
            arabicCulture.DateTimeFormat.PMDesignator = "مساء";

            CreateMap<Payment, PaymentDTO>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.PaymentId))
                .ForMember(dest => dest.PaymentIntentId, opt => opt.MapFrom(src => src.PaymentIntentId))
                .ForMember(dest => dest.IsRefunded, opt => opt.MapFrom(src => src.IsRefunded))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.amount))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method))
                .ForMember(dest => dest.time, opt => opt.MapFrom(src => src.Date.ToString("dddd yyyy-MM-dd HH:mm tt", arabicCulture)))
                .ForMember(dest => dest.SeatNumbers, opt => opt.MapFrom(src => src.booking.seats.Select(s => s.Number).ToList()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.booking.UserId))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.booking.user.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.booking.user.FirstName != null ? src.booking.user.FirstName : src.booking.GuestFirstName))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.booking.trip.company.Name));
        }
    }
}
