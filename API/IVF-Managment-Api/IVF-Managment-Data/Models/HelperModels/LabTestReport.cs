namespace IVF_Managment_Api.Models.HelperModels;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



    [Table("LabTestReports")]
    public class LabTestReport
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        [Required]
        public Guid TechnicianId { get; set; }
        public virtual LabTechnician Technician { get; set; }

        [Required]
        public string ResultValuesJson { get; set; } 
        public string ReferenceRanges { get; set; }
        
        public bool IsAbnormal { get; set; } 
        public bool IsCritical { get; set; } 

        [MaxLength(500)]
        public string ResultFileUrl { get; set; }

        public Guid? OrderId { get; set; }
        public virtual LabTestOrder? Order { get; set; }

        public bool IsReleasedToPatient { get; set; }
        public DateTime? ReleasedAt { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
