using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SampleCustodyController : ControllerBase
{
    private readonly ISampleCustodyService _service;

    public SampleCustodyController(ISampleCustodyService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<SampleCustodyEventResponseDto>> RecordEvent(CreateSampleCustodyEventDto dto)
    {
        try
        {
            var result = await _service.RecordEventAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{sampleIdentifier}")]
    public async Task<ActionResult<IEnumerable<SampleCustodyEventResponseDto>>> GetBySample(string sampleIdentifier)
    {
        var result = await _service.GetBySampleAsync(sampleIdentifier);
        return Ok(result);
    }
}