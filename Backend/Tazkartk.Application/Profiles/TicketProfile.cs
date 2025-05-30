using AutoMapper;
using System.Globalization;
using Tazkartk.Application.DTO;
using Tazkartk.Application.Extensions;
using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Profiles
{
    public class TicketProfile : Profile
    {
        private const char RightToLeftCharacter = (char)0x200F;
        public TicketProfile()
        {
            CreateMap<Booking, TicketDTO>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.user.FirstName != null ? src.user.FirstName + " " + src.user.LastName : src.GuestFirstName + " " + src.GuestLastName))
                .ForMember(dest => dest.userEmail, opt => opt.MapFrom(src => src.user.Email))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.trip.company.Name))
                 .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => src.trip.Date.ToArabicDateString()))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => RightToLeftCharacter + src.trip.Time.ToArabicTimeString()))
                .ForMember(dest => dest.DepartureDay, opt => opt.MapFrom(src => src.trip.Date.ToArabicDayString()))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.trip.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.trip.To))
                .ForMember(dest => dest.IsCanceled, opt => opt.MapFrom(src => src.IsCanceled))
                .ForMember(dest => dest.SeatsNumbers, opt => opt.MapFrom(src => src.seats.Select(s => s.Number).ToList()))
                .ForMember(dest => dest.CompanyLogoUrl,
                   opt => opt.MapFrom(src => src.trip.company.Logo));

            CreateMap<Booking, PassengerDetailsDTO>()
                 //.ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.BookingId))
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.user.Id))
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.user.FirstName != null ? src.user.FirstName : src.GuestFirstName))
                  .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.user.LastName != null ? src.user.LastName : src.GuestLastName))
                 .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.user.Email))
                  .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.user.LastName != null ? src.user.PhoneNumber : src.GuestPhoneNumber))
                   .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.seats.Select(s => s.Number).ToList()));


        }
    }
}
                //.ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => src.trip.Date.ToString("yyyy-MM-dd", arabicCulture)))