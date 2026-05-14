using System.ComponentModel.DataAnnotations;

namespace IVF_Managment_Api.Dtos;

public class CreateRoomDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [MaxLength(100)]
    public string Type { get; set; }

    public int Capacity { get; set; }
}

public class UpdateRoomDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(100)]
    public string? Type { get; set; }

    public int? Capacity { get; set; }
    public bool? IsActive { get; set; }
}

public class RoomResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
}

public class AssignRoomDto
{
    [Required]
    public Guid AppointmentId { get; set; }

    [Required]
    public Guid RoomId { get; set; }
}