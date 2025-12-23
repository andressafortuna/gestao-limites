using AutoMapper;
using FraudSys.Application.DTOs;
using FraudSys.Domain.Entities;

namespace FraudSys.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AccountLimit, AccountLimitResponseDto>()
                .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document.Value))
                .ForMember(dest => dest.PixLimit, opt => opt.MapFrom(src => src.PixLimit.Value));

            CreateMap<PixTransaction, PixTransactionResponseDto>()
                .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document.Value))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Value))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
