namespace IVF_Managment_Api.Models.HelperModels;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


    [Table("EmbryoObservations")]
    public  class EmbryoObservation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EmbryoId { get; set; }
        public virtual Embryo Embryo { get; set; }

        [Required]
        public Guid TechnicianId { get; set; }
        public virtual LabTechnician Technician { get; set; }

        [Required]
        public int DayOfDevelopment { get; set; }

        [Required]
        public int CellCount { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal FragmentationPercentage { get; set; }

        [Required]
        [MaxLength(20)]
        public string MorphologyGrade { get; set; } // e.g., Gardner Grade [cite: 61]

        public string FreeTextNotes { get; set; }

        [MaxLength(500)]
        public string MicroscopyImageUrl { get; set; } // Max 10 MB limit [cite: 61]

        public DateTime ObservationDate { get; set; } = DateTime.UtcNow;
    }
