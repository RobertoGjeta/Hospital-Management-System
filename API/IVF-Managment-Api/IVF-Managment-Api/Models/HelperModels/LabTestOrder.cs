using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IvfClinic.Models;

namespace IVF_Managment_Api.Models.HelperModels;


    [Table("LabTestOrders")]
    public class LabTestOrder
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        [Required]
        public Guid RequestingDoctorId { get; set; }
        public virtual Doctor RequestingDoctor { get; set; }

        public Guid? FulfillingTechnicianId { get; set; }
        public virtual LabTechnician FulfillingTechnician { get; set; }

        [Required]
        public string TestCategory { get; set; }
        public TestPriority Priority { get; set; }
        public string ClinicalInstructions { get; set; }

        public TestStatus Status { get; set; } = TestStatus.Pending;

     
        public string ResultValuesJson { get; set; } // Stores structured test data
        public string ReferenceRanges { get; set; }
        public bool IsAbnormal { get; set; } // Auto-flagged if out of range [cite: 53]
        public bool IsCritical { get; set; } // Triggers urgent alerts [cite: 239]
        public string ResultFileUrl { get; set; } // Link to uploaded PDF [cite: 53]

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UploadedAt { get; set; }
        public DateTime? ReviewedAt { get; set; } // Timestamped when doctor acknowledges [cite: 239]
    }
