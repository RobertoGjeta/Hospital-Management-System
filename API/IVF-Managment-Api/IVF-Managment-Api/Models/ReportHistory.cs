using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IVFClinic.Models
{
    public class ReportHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string Type { get; set; } = string.Empty;
        // "Occupancy", "Revenue", "Utilization"

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Filters { get; set; }  // JSON serialized ReportFilters

        [Column(TypeName = "nvarchar(max)")]
        public string? ResultSnapshot { get; set; }  // Optional JSON of the report data

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public Guid? GeneratedById { get; set; }
        [ForeignKey(nameof(GeneratedById))]
        public Admin? GeneratedBy { get; set; }
    }
}
