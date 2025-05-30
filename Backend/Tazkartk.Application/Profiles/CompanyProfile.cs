using AutoMapper;
using Tazkartk.Application.DTO.CompanyDTOs;
using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Profiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<Company, CompanyRegisterDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim()))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber.Trim()))
                .ForMember(dest => dest.city, opt => opt.MapFrom(src => src.City.Trim()))
                .ForMember(dest => dest.street, opt => opt.MapFrom(src => src.Street.Trim()))
                .ReverseMap();

            CreateMap<Company, CompanyDTO>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
               .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
               .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Logo))
               .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
               .ReverseMap();





        }
    }
}
