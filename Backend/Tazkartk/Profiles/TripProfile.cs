using AutoMapper;
using System.Globalization;
using Tazkartk.DTO.TripDTOs;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Profiles
{
    public class TripProfile:Profile
    {
        private const char RightToLeftCharacter = (char)0x200F;
        private readonly CultureInfo arabicCulture;
        public TripProfile()
        {
            var arabicCulture = new CultureInfo("ar-SA");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            arabicCulture.DateTimeFormat.AMDesignator = "صباحا";
            arabicCulture.DateTimeFormat.PMDesignator = "مساء";

            CreateMap<Trip, TripDtos>()
                .ForMember(dest => dest.TripId, opt => opt.MapFrom(src => src.TripId))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(dest => dest.Avaliblility, opt => opt.MapFrom(src => src.Avaliblility))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DepartureDay, opt => opt.MapFrom(src => src.Date.ToString("dddd", arabicCulture)))
                .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => src.Date.ToString("yyyy-MM-dd", arabicCulture)))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => RightToLeftCharacter + src.Time.ToString("hh:mm tt", arabicCulture)))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.company.Name));

            CreateMap<Trip, TripDTO>()
               .ForMember(dest => dest.TripId, opt => opt.MapFrom(src => src.TripId))
               .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
               .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
               .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class))
               .ForMember(dest => dest.Avaliblility, opt => opt.MapFrom(src => src.Avaliblility))
               .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
               .ForMember(dest => dest.DepartureDay, opt => opt.MapFrom(src => src.Date.ToString("dddd", arabicCulture)))
               .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => src.Date.ToString("yyyy-MM-dd", arabicCulture)))
               .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => RightToLeftCharacter + src.Time.ToString("hh:mm tt", arabicCulture)))
               .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.company.Name))
               .ForMember(dest => dest.BookedSeats, opt => opt.MapFrom(src => src.bookings.SelectMany(b => b.seats.Where(s => s.State == SeatState.Booked).Select(s => s.Number)).ToList()));


            CreateMap<CreateTripDtos, Trip>();
        }
    }
}
