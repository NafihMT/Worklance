using AutoMapper;
using Worklance.Application.DTOs.FreelancerProfiles;
using Worklance.Domain.Entities;

namespace Worklance.Application.Mapping.FreelancerProfileMapping;

public class FreelancerProfileMappingProfile : Profile
{
    public FreelancerProfileMappingProfile()
    {
        // Entity to DTO
        CreateMap<FreelancerProfile, FreelancerProfileDto>();
        CreateMap<FreelancerEducation, FreelancerEducationDto>();
        CreateMap<FreelancerCertification, FreelancerCertificationDto>();
        CreateMap<FreelancerPortfolio, FreelancerPortfolioDto>();
        CreateMap<FreelancerLanguage, FreelancerLanguageDto>();
        CreateMap<Skill, SkillDto>();

        // DTO to Entity
        CreateMap<CreateEducationDto, FreelancerEducation>();
        CreateMap<CreateCertificationDto, FreelancerCertification>();
        CreateMap<CreatePortfolioDto, FreelancerPortfolio>();
        CreateMap<CreateLanguageDto, FreelancerLanguage>();
    }
}

