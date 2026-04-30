using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IvfClinic.Models;

namespace IVF_Managment_Api.Models.HelperModels;

[Table("IvfCycles")]
    public class IvfCycle
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        [Required]
        public Guid AssignedDoctorId { get; set; }
        public virtual Doctor AssignedDoctor { get; set; }

        public CyclePhase CurrentPhase { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        public string CycleProtocol { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Embryo> Embryos { get; set; }
    }

    [Table("Embryos")]
    public class Embryo
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid IvfCycleId { get; set; }
        public virtual IvfCycle IvfCycle { get; set; }

        public string EmbryoIdentifier { get; set; }
        public EmbryoStatus Status { get; set; }

     
        public string StorageTank { get; set; }
        public string StorageCane { get; set; }
        public string VitrificationMethod { get; set; }
        
        public string DoctorInstructions { get; set; } // Free text for lab instructions [cite: 35]

        public virtual ICollection<EmbryoObservation> Observations { get; set; }
    }

