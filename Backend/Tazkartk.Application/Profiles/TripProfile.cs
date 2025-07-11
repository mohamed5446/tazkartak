﻿using AutoMapper;
using System.Globalization;
using Tazkartk.Application.DTO.TripDTOs;
using Tazkartk.Application.Extensions;
using Tazkartk.Domain.Models;
using Tazkartk.Domain.Models.Enums;
namespace Tazkartk.Application.Profiles
{
    public class TripProfile : Profile
    {
        private const char RightToLeftCharacter = (char)0x200F;
        public TripProfile()
        {


            CreateMap<Trip, TripDtos>()
                .ForMember(dest => dest.TripId, opt => opt.MapFrom(src => src.TripId))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(dest => dest.Avaliblility, opt => opt.MapFrom(src => src.Avaliblility))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DepartureDay, opt => opt.MapFrom(src => src.Date.ToArabicDayString()))
                .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => src.Date.ToArabicDateString()))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => RightToLeftCharacter + src.Time.ToArabicTimeString()))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.company.Name));

            CreateMap<Trip, TripDTO>()
               .ForMember(dest => dest.TripId, opt => opt.MapFrom(src => src.TripId))
               .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
               .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
               .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class))
               .ForMember(dest => dest.Avaliblility, opt => opt.MapFrom(src => src.Avaliblility))
               .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
               .ForMember(dest => dest.DepartureDay, opt => opt.MapFrom(src => src.Date.ToArabicDayString()))
               .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => src.Date.ToArabicDateString()))
               .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => RightToLeftCharacter + src.Time.ToArabicTimeString()))
               .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.company.Name))
               .ForMember(dest => dest.BookedSeats, opt => opt.MapFrom(src => src.bookings.SelectMany(b => b.seats.Where(s => s.State == SeatState.Booked).Select(s => s.Number)).ToList()));


            CreateMap<CreateTripDtos, Trip>();
        }
    }
}
