using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IVF_Managment_Api.Models.BaseModel;
using IVF_Managment_Api.Models.HelperModels;

namespace IVF_Managment_Api.Models;

[Table("Administrators")]
public class Administrator : User
{
    [MaxLength(100)]
    public string? Department { get; set; }

    
    public virtual ICollection<Bill> GeneratedBills { get; set; }
    public virtual ICollection<Payment> RecordedPayments { get; set; }
}