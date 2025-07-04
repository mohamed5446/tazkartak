﻿using AutoMapper;
using Tazkartk.Application.DTO.AccontDTOs;
using Tazkartk.Application.DTO.UserDTOs;
using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDetails>()
                 .ForMember(dest => dest.phoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                 .ForMember(des => des.PhotoUrl, opt => opt.MapFrom(src => src.photo));

            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email.Trim()))
                
                .ReverseMap();

            CreateMap<User, UserDetailsDTO>();
            //.ForMember(dest => dest.photo, opt => opt.Ignore());
            //.ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore());
        }
    }
}
