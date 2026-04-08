using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models;

namespace IVF_Managment_Api.Mappings;

public static class LabTechnicianMapper
{
    public static LabTechnician ToEntity(CreateLabTechnicianDto dto)
    {
        return new LabTechnician
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = string.Empty,
            NationalId = dto.NationalId,
            Phone = dto.Phone,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            ProfileImageUrl = dto.ProfileImageUrl,
            EmployeeId = dto.EmployeeId,
            HireDate = dto.HireDate,
            Specialization = dto.Specialization,
            LicenseNumber = dto.LicenseNumber,
            Qualifications = dto.Qualifications
        };
    }

    public static LabTechnician ToEntity(UpdateLabTechnicianDto dto)
    {
        return new LabTechnician
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            NationalId = dto.NationalId,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            ProfileImageUrl = dto.ProfileImageUrl,
            EmployeeId = dto.EmployeeId,
            HireDate = dto.HireDate,
            Specialization = dto.Specialization,
            LicenseNumber = dto.LicenseNumber,
            Qualifications = dto.Qualifications
        };
    }

    public static LabTechnicianResponseDto ToResponse(LabTechnician entity)
    {
        return new LabTechnicianResponseDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Username = entity.Username,
            Email = entity.Email,
            NationalId = entity.NationalId,
            Phone = entity.Phone,
            Gender = entity.Gender,
            DateOfBirth = entity.DateOfBirth,
            ProfileImageUrl = entity.ProfileImageUrl,
            EmployeeId = entity.EmployeeId,
            HireDate = entity.HireDate,
            Specialization = entity.Specialization,
            LicenseNumber = entity.LicenseNumber,
            Qualifications = entity.Qualifications,
            IsActive = entity.IsActive,
            LastLoginAt = entity.LastLoginAt,
            PasswordChangedAt = entity.PasswordChangedAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
