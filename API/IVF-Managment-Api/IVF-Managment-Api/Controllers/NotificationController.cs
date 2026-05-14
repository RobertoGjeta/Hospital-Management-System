using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationController(INotificationService service) => _service = service;

    [HttpGet("unread/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<NotificationResponseDto>>> GetUnread(Guid userId)
    {
        var result = await _service.GetUnreadAsync(userId);
        return Ok(result);
    }

    [HttpPut("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId)
    {
        var success = await _service.MarkAsReadAsync(notificationId);
        if (!success) return NotFound();
        return NoContent();
    }
}