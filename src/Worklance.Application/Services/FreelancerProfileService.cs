using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Worklance.Application.DTOs.FreelancerProfiles;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Application.Interfaces.Services;
using Worklance.Domain.Entities;
using Worklance.Domain.Enums;

namespace Worklance.Application.Services;

public class FreelancerProfileService : IFreelancerProfileService
{
    private readonly IFreelancerProfileRepository _profileRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public FreelancerProfileService(
        IFreelancerProfileRepository profileRepository,
        ISkillRepository skillRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IMapper mapper)
    {
        _profileRepository = profileRepository;
        _skillRepository = skillRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public async Task<FreelancerProfileDto> CreateProfileAsync(string userId, CreateFreelancerProfileDto dto)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        // Validate: One profile per user
        if (await _profileRepository.HasProfileAsync(userId))
        {
            throw new InvalidOperationException("Freelancer profile already exists for this user.");
        }

        // Validate: Experience
        ValidateExperience(dto.IsExperienced, dto.ExperienceYears, dto.ExperienceMonths);
        ValidateSocialLinks(dto.GitHubUrl, dto.LinkedInUrl, dto.PortfolioWebsiteUrl, dto.TwitterUrl);

        var profile = new FreelancerProfile
        {
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Country = dto.Country,
            State = dto.State,
            City = dto.City,
            Address = dto.Address,
            ProfessionalTitle = dto.ProfessionalTitle,
            AboutMe = dto.AboutMe,
            PrimaryTechnologyStack = dto.PrimaryTechnologyStack,
            Specialization = dto.Specialization,
            IsExperienced = dto.IsExperienced,
            ExperienceYears = dto.IsExperienced ? dto.ExperienceYears : null,
            ExperienceMonths = dto.IsExperienced ? dto.ExperienceMonths : null,
            Availability = dto.Availability,
            WorkPreference = dto.WorkPreference,
            HourlyRate = dto.HourlyRate,
            GitHubUrl = dto.GitHubUrl,
            LinkedInUrl = dto.LinkedInUrl,
            PortfolioWebsiteUrl = dto.PortfolioWebsiteUrl,
            TwitterUrl = dto.TwitterUrl,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            LastModifiedAt = DateTime.UtcNow,
            LastModifiedBy = userId
        };

        // Handle Skills
        await ProcessSkillsAsync(profile, dto.Skills);

        // Handle Educations
        if (dto.Educations != null)
        {
            foreach (var edu in dto.Educations)
            {
                profile.Educations.Add(_mapper.Map<FreelancerEducation>(edu));
            }
        }

        // Handle Certifications
        if (dto.Certifications != null)
        {
            foreach (var cert in dto.Certifications)
            {
                profile.Certifications.Add(_mapper.Map<FreelancerCertification>(cert));
            }
        }

        // Handle Portfolios
        if (dto.Portfolios != null)
        {
            foreach (var port in dto.Portfolios)
            {
                profile.Portfolios.Add(_mapper.Map<FreelancerPortfolio>(port));
            }
        }

        // Handle Languages
        if (dto.Languages != null)
        {
            foreach (var lang in dto.Languages)
            {
                profile.Languages.Add(_mapper.Map<FreelancerLanguage>(lang));
            }
        }

        await _profileRepository.AddAsync(profile);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FreelancerProfileDto>(profile);
    }

    public async Task<FreelancerProfileDto> UpdateProfileAsync(string userId, UpdateFreelancerProfileDto dto)
    {
        var profile = await _profileRepository.GetProfileWithDetailsAsync(userId);
        if (profile == null)
        {
            throw new KeyNotFoundException("Profile not found.");
        }

        // Validate: Experience
        ValidateExperience(dto.IsExperienced, dto.ExperienceYears, dto.ExperienceMonths);
        ValidateSocialLinks(dto.GitHubUrl, dto.LinkedInUrl, dto.PortfolioWebsiteUrl, dto.TwitterUrl);

        profile.FirstName = dto.FirstName;
        profile.LastName = dto.LastName;
        profile.Username = dto.Username;
        profile.Email = dto.Email;
        profile.PhoneNumber = dto.PhoneNumber;
        profile.DateOfBirth = dto.DateOfBirth;
        profile.Gender = dto.Gender;
        profile.Country = dto.Country;
        profile.State = dto.State;
        profile.City = dto.City;
        profile.Address = dto.Address;
        profile.ProfessionalTitle = dto.ProfessionalTitle;
        profile.AboutMe = dto.AboutMe;
        profile.PrimaryTechnologyStack = dto.PrimaryTechnologyStack;
        profile.Specialization = dto.Specialization;
        profile.IsExperienced = dto.IsExperienced;
        profile.ExperienceYears = dto.IsExperienced ? dto.ExperienceYears : null;
        profile.ExperienceMonths = dto.IsExperienced ? dto.ExperienceMonths : null;

        profile.Availability = dto.Availability;
        profile.WorkPreference = dto.WorkPreference;
        profile.HourlyRate = dto.HourlyRate;
        profile.GitHubUrl = dto.GitHubUrl;
        profile.LinkedInUrl = dto.LinkedInUrl;
        profile.PortfolioWebsiteUrl = dto.PortfolioWebsiteUrl;
        profile.TwitterUrl = dto.TwitterUrl;
        profile.LastModifiedAt = DateTime.UtcNow;
        profile.LastModifiedBy = userId;

        // Process Skills
        profile.Skills.Clear();
        await ProcessSkillsAsync(profile, dto.Skills);

        // Process Educations
        profile.Educations.Clear();
        if (dto.Educations != null)
        {
            foreach (var edu in dto.Educations)
            {
                profile.Educations.Add(_mapper.Map<FreelancerEducation>(edu));
            }
        }

        // Process Certifications
        profile.Certifications.Clear();
        if (dto.Certifications != null)
        {
            foreach (var cert in dto.Certifications)
            {
                profile.Certifications.Add(_mapper.Map<FreelancerCertification>(cert));
            }
        }

        // Process Portfolios
        profile.Portfolios.Clear();
        if (dto.Portfolios != null)
        {
            foreach (var port in dto.Portfolios)
            {
                profile.Portfolios.Add(_mapper.Map<FreelancerPortfolio>(port));
            }
        }

        // Process Languages
        profile.Languages.Clear();
        if (dto.Languages != null)
        {
            foreach (var lang in dto.Languages)
            {
                profile.Languages.Add(_mapper.Map<FreelancerLanguage>(lang));
            }
        }

        _profileRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FreelancerProfileDto>(profile);
    }

    public async Task<FreelancerProfileDto?> GetProfileByUserIdAsync(string userId)
    {
        var profile = await _profileRepository.GetProfileWithDetailsAsync(userId);
        return profile == null ? null : _mapper.Map<FreelancerProfileDto>(profile);
    }

    public async Task<FreelancerProfileDto?> GetProfileByIdAsync(int id)
    {
        var profile = await _profileRepository.GetProfileWithDetailsByIdAsync(id);
        return profile == null ? null : _mapper.Map<FreelancerProfileDto>(profile);
    }

    public async Task DeleteProfileAsync(string userId)
    {
        var profile = await _profileRepository.GetProfileWithDetailsAsync(userId);
        if (profile == null)
        {
            throw new KeyNotFoundException("Profile not found.");
        }

        if (!string.IsNullOrEmpty(profile.ResumeUrl))
        {
            _fileStorageService.DeleteFile(profile.ResumeUrl);
        }

        _profileRepository.Delete(profile);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<string> UploadResumeAsync(string userId, Stream fileStream, string fileName, long fileSize)
    {
        var profile = await _profileRepository.GetProfileWithDetailsAsync(userId);
        if (profile == null)
        {
            throw new KeyNotFoundException("Profile not found. Resume must be uploaded to an existing profile.");
        }

        // Validate resume file size (Max 5MB)
        if (fileSize > 5 * 1024 * 1024)
        {
            throw new ArgumentException("File size exceeds the 5MB limit.");
        }

        // Validate resume extension
        var extension = Path.GetExtension(fileName)?.ToLower();
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Invalid file type. Only PDF, DOC, and DOCX are allowed.");
        }

        var oldResumeUrl = profile.ResumeUrl;

        // Save new resume first
        var relativePath = await _fileStorageService.SaveFileAsync(fileStream, fileName, "resumes");
        profile.ResumeUrl = relativePath;
        profile.LastModifiedAt = DateTime.UtcNow;
        profile.LastModifiedBy = userId;

        _profileRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync();

        // Delete old resume file only after database updates successfully
        if (!string.IsNullOrEmpty(oldResumeUrl))
        {
            _fileStorageService.DeleteFile(oldResumeUrl);
        }

        return relativePath;
    }

    public async Task DeleteResumeAsync(string userId)
    {
        var profile = await _profileRepository.GetProfileWithDetailsAsync(userId);
        if (profile == null)
        {
            throw new KeyNotFoundException("Profile not found.");
        }

        if (string.IsNullOrEmpty(profile.ResumeUrl))
        {
            throw new InvalidOperationException("No resume associated with this profile.");
        }

        var oldResumeUrl = profile.ResumeUrl;
        profile.ResumeUrl = null;
        profile.LastModifiedAt = DateTime.UtcNow;
        profile.LastModifiedBy = userId;

        _profileRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync();

        _fileStorageService.DeleteFile(oldResumeUrl);
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)> DownloadResumeAsync(string userId)
    {
        var profile = await _profileRepository.GetProfileWithDetailsAsync(userId);
        if (profile == null || string.IsNullOrEmpty(profile.ResumeUrl))
        {
            throw new FileNotFoundException("Resume not found.");
        }

        var stream = await _fileStorageService.GetFileStreamAsync(profile.ResumeUrl);
        var extension = Path.GetExtension(profile.ResumeUrl).ToLower();
        var contentType = extension switch
        {
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".doc" => "application/msword",
            _ => "application/octet-stream"
        };

        var fileName = Path.GetFileName(profile.ResumeUrl);

        return (stream, contentType, fileName);
    }

    public async Task<FreelancerProfileDto> UpdateSocialLinksAsync(string userId, UpdateSocialLinksDto dto)
    {
        var profile = await _profileRepository.GetProfileWithDetailsAsync(userId);
        if (profile == null)
        {
            throw new KeyNotFoundException("Profile not found.");
        }

        ValidateSocialLinks(dto.GitHubUrl, dto.LinkedInUrl, dto.PortfolioWebsiteUrl, dto.TwitterUrl);

        profile.GitHubUrl = dto.GitHubUrl;
        profile.LinkedInUrl = dto.LinkedInUrl;
        profile.PortfolioWebsiteUrl = dto.PortfolioWebsiteUrl;
        profile.TwitterUrl = dto.TwitterUrl;
        profile.LastModifiedAt = DateTime.UtcNow;
        profile.LastModifiedBy = userId;

        _profileRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FreelancerProfileDto>(profile);
    }

    private void ValidateExperience(bool isExperienced, int? years, int? months)
    {
        if (isExperienced)
        {
            var y = years ?? 0;
            var m = months ?? 0;
            if (y < 0 || m < 0)
            {
                throw new ArgumentException("Experience years and months cannot be negative.");
            }
            if (y == 0 && m == 0)
            {
                throw new ArgumentException("Years or months of experience is required for experienced freelancers.");
            }
            if (m > 11)
            {
                throw new ArgumentException("Experience months cannot exceed 11.");
            }
        }
    }

    private void ValidateSocialLinks(string? gitHubUrl, string? linkedInUrl, string? portfolioWebsiteUrl, string? twitterUrl)
    {
        ValidateUrl(gitHubUrl, "GitHub");
        ValidateUrl(linkedInUrl, "LinkedIn");
        ValidateUrl(portfolioWebsiteUrl, "Portfolio Website");
        ValidateUrl(twitterUrl, "Twitter");
    }

    private void ValidateUrl(string? url, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) || 
            (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException($"Invalid URL format for {fieldName}. Must start with http:// or https://");
        }
    }

    private async Task ProcessSkillsAsync(FreelancerProfile profile, List<string> skillNames)
    {
        if (skillNames == null || !skillNames.Any())
        {
            throw new ArgumentException("At least one skill is required.");
        }

        foreach (var skillName in skillNames.Distinct())
        {
            var trimmed = skillName.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            var skill = await _skillRepository.GetByNameAsync(trimmed);
            if (skill == null)
            {
                skill = new Skill { Name = trimmed };
                await _skillRepository.AddAsync(skill);
            }
            profile.Skills.Add(skill);
        }
    }
}
