using IvfClinic.Models;

namespace IVF_Managment_Api.Models.HelperModels;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IVF_Managment_Api.Models.HelperModels;



    [Table("ChainOfCustodyLogs")]
    public class ChainOfCustodyLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TechnicianId { get; set; }
        public virtual LabTechnician Technician { get; set; }

        [Required]
        [MaxLength(100)]
        public string SampleIdentifier { get; set; } // ID for Sperm, Egg, or Embryo sample

        [Required]
        public CustodyEventType EventType { get; set; } // Received, Processed, Stored, Transferred, Discarded [cite: 39]

        public string DestinationRecipient { get; set; } // Required if EventType == Transferred [cite: 92, 103]

        public string DisposalReason { get; set; } // Required if EventType == Discarded [cite: 93, 102]

        public string AdditionalNotes { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Immutable timestamp [cite: 39]
    }