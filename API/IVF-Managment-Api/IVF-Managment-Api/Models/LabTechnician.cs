using System.ComponentModel.DataAnnotations;

namespace IVFClinic.Models
{
    public class LabTechnician : User
    {
        [MaxLength(50)]
        public string? TechnicianId { get; set; }

        [MaxLength(200)]
        public string? LabSection { get; set; }

        [MaxLength(500)]
        public string? Certifications { get; set; }

        public DateTime? HireDate { get; set; }
    }
}
