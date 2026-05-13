using IVF_Managment_Api.Data;
using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Models;
using IvfClinic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IVF_Managment_Api.Services;

public class AuthService : IAuthService
{
    private readonly IvfDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(IvfDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
    {
        var passwordHash = HashPassword(dto.Password);
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                (u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail) &&
                u.PasswordHash == passwordHash &&
                u.IsActive);

        if (user is null) return null;

        return new TokenResponseDto
        {
            Token = GenerateToken(user.Id, user.Username, user.Email, user.Role),
            Role = user.Role.ToString(),
            UserId = user.Id,
            Username = user.Username,
            ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpirationMinutes"]!))
        };
    }

    public async Task<(PatientResponseDto? Result, string? Error)> RegisterPatientAsync(CreatePatientDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return (null, "A user with this email already exists.");

        if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
            return (null, "A user with this username already exists.");

        var seq = await _db.Patients.CountAsync() + 1;

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = UserRole.Patient,
            PatientSystemId = $"PAT-{seq:D6}",
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            NationalIdNumber = dto.NationalIdNumber,
            Address = dto.Address,
            BillingType = dto.BillingType,
            InsuranceProvider = dto.InsuranceProvider,
            InsurancePolicyNumber = dto.InsurancePolicyNumber,
            MedicalHistoryNotes = dto.MedicalHistoryNotes,
            KnownAllergies = dto.KnownAllergies,
            AssignedDoctorId = dto.AssignedDoctorId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        return (MapPatient(patient), null);
    }

    public async Task<(RegisteredUserResponseDto? Result, string? Error)> AdminRegisterAsync(AdminRegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return (null, "A user with this email already exists.");

        if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
            return (null, "A user with this username already exists.");

        var hash = HashPassword(dto.Password);
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        RegisteredUserResponseDto response;

        switch (dto.Role)
        {
            case UserRole.Doctor:
                if (string.IsNullOrWhiteSpace(dto.Specialization))
                    return (null, "Specialization is required for Doctor.");
                if (string.IsNullOrWhiteSpace(dto.LicenseNumber))
                    return (null, "LicenseNumber is required for Doctor.");

                var doctor = new Doctor
                {
                    Id = id, FirstName = dto.FirstName, LastName = dto.LastName,
                    Username = dto.Username, Email = dto.Email, PasswordHash = hash,
                    PhoneNumber = dto.PhoneNumber, Role = UserRole.Doctor,
                    Specialization = dto.Specialization, LicenseNumber = dto.LicenseNumber,
                    Qualifications = dto.Qualifications, CreatedAt = now, IsActive = true
                };
                _db.Doctors.Add(doctor);
                response = MapBase(doctor);
                response.Specialization = doctor.Specialization;
                response.LicenseNumber = doctor.LicenseNumber;
                response.Qualifications = doctor.Qualifications;
                break;

            case UserRole.LabTechnician:
                if (string.IsNullOrWhiteSpace(dto.TechnicianId))
                    return (null, "TechnicianId is required for LabTechnician.");

                var tech = new LabTechnician
                {
                    Id = id, FirstName = dto.FirstName, LastName = dto.LastName,
                    Username = dto.Username, Email = dto.Email, PasswordHash = hash,
                    PhoneNumber = dto.PhoneNumber, Role = UserRole.LabTechnician,
                    TechnicianId = dto.TechnicianId, CreatedAt = now, IsActive = true
                };
                _db.LabTechnicians.Add(tech);
                response = MapBase(tech);
                response.TechnicianId = tech.TechnicianId;
                break;

            case UserRole.Administrator:
                var admin = new Administrator
                {
                    Id = id, FirstName = dto.FirstName, LastName = dto.LastName,
                    Username = dto.Username, Email = dto.Email, PasswordHash = hash,
                    PhoneNumber = dto.PhoneNumber, Role = UserRole.Administrator,
                    Department = dto.Department, CreatedAt = now, IsActive = true
                };
                _db.Administrators.Add(admin);
                response = MapBase(admin);
                response.Department = admin.Department;
                break;

            case UserRole.Patient:
                if (dto.DateOfBirth is null) return (null, "DateOfBirth is required for Patient.");
                if (dto.Gender is null) return (null, "Gender is required for Patient.");
                if (string.IsNullOrWhiteSpace(dto.NationalIdNumber))
                    return (null, "NationalIdNumber is required for Patient.");

                var seq = await _db.Patients.CountAsync() + 1;
                var patient = new Patient
                {
                    Id = id, FirstName = dto.FirstName, LastName = dto.LastName,
                    Username = dto.Username, Email = dto.Email, PasswordHash = hash,
                    PhoneNumber = dto.PhoneNumber, Role = UserRole.Patient,
                    PatientSystemId = $"PAT-{seq:D6}", DateOfBirth = dto.DateOfBirth.Value,
                    Gender = dto.Gender.Value, NationalIdNumber = dto.NationalIdNumber,
                    Address = dto.Address, BillingType = dto.BillingType ?? BillingType.SelfPay,
                    InsuranceProvider = dto.InsuranceProvider, InsurancePolicyNumber = dto.InsurancePolicyNumber,
                    MedicalHistoryNotes = dto.MedicalHistoryNotes, KnownAllergies = dto.KnownAllergies,
                    AssignedDoctorId = dto.AssignedDoctorId, CreatedAt = now, IsActive = true
                };
                _db.Patients.Add(patient);
                response = MapBase(patient);
                response.PatientSystemId = patient.PatientSystemId;
                response.NationalIdNumber = patient.NationalIdNumber;
                break;

            default:
                return (null, "Invalid role.");
        }

        await _db.SaveChangesAsync();
        return (response, null);
    }

    private string GenerateToken(Guid userId, string username, string email, UserRole role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpirationMinutes"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static RegisteredUserResponseDto MapBase(IVF_Managment_Api.Models.BaseModel.User u) => new()
    {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Username = u.Username,
        Email = u.Email,
        PhoneNumber = u.PhoneNumber,
        Role = u.Role.ToString(),
        CreatedAt = u.CreatedAt
    };

    private static PatientResponseDto MapPatient(Patient p) => new()
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Username = p.Username,
        Email = p.Email,
        PhoneNumber = p.PhoneNumber,
        PatientSystemId = p.PatientSystemId,
        DateOfBirth = p.DateOfBirth,
        Gender = p.Gender,
        NationalIdNumber = p.NationalIdNumber,
        Address = p.Address,
        BillingType = p.BillingType,
        InsuranceProvider = p.InsuranceProvider,
        InsurancePolicyNumber = p.InsurancePolicyNumber,
        MedicalHistoryNotes = p.MedicalHistoryNotes,
        KnownAllergies = p.KnownAllergies,
        AssignedDoctorId = p.AssignedDoctorId,
        CreatedAt = p.CreatedAt,
        IsActive = p.IsActive
    };

    private static string HashPassword(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
}
