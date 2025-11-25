using AutoMapper;
using ULIP_proj.Models;
using ULIP_proj.DTOs;

namespace ULIP_proj.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterDto, User>();

            CreateMap<Policy, PolicyDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));

            CreateMap<Policy, PolicyProposalDto>()
                .ForMember(dest => dest.AgentName, opt => opt.MapFrom(src => src.AgentId.HasValue ? "Agent User" : "No Agent"));

            CreateMap<CreatePolicyProposalDto, Policy>()
                .ForMember(dest => dest.PolicyStatus, opt => opt.MapFrom(src => Enums.PolicyStatus.PendingAcceptance))
                .ForMember(dest => dest.ProposedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Fund, FundDto>();

            CreateMap<Sale, SaleDto>();
            CreateMap<CreateSaleDto, Sale>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enums.SaleStatus.Lead));
            
            CreateMap<Approval, ApprovalDto>();
        }
    }
}
