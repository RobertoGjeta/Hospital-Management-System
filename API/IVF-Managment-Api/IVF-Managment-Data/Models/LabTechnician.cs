using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IVF_Managment_Api.Models.BaseModel;
using IVF_Managment_Api.Models.HelperModels;

namespace IVF_Managment_Api.Models;

[Table("LabTechnicians")]
public class LabTechnician : User
{
    [Required]
    [MaxLength(50)]
    public string TechnicianId { get; set; } // Used for timestamped audit trails [cite: 1535]

    // Navigation Properties
    public virtual ICollection<LabTestReport> UploadedTests { get; set; }
    public virtual ICollection<EmbryoObservation> EmbryoObservations { get; set; }
    public virtual ICollection<ChainOfCustodyLog> CustodyLogs { get; set; }
}