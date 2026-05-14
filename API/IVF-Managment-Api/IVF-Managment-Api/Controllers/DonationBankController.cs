using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DonationBankController : ControllerBase
{
    private readonly IDonationBankService _service;

    public DonationBankController(IDonationBankService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<DonationSampleResponseDto>> Create(CreateDonationSampleDto dto)
    {
        var result = await _service.CreateSampleAsync(dto);
        return CreatedAtAction(nameof(GetByDonor), new { donorId = result.DonorId }, result);
    }

    [HttpGet("donor/{donorId:guid}")]
    public async Task<ActionResult<IEnumerable<DonationSampleResponseDto>>> GetByDonor(Guid donorId)
    {
        var result = await _service.GetByDonorAsync(donorId);
        return Ok(result);
    }

    [HttpGet("assignable")]
    public async Task<ActionResult<IEnumerable<DonationSampleResponseDto>>> GetAssignable()
    {
        var result = await _service.GetAssignableAsync();
        return Ok(result);
    }

    [HttpPut("{sampleId:guid}/screening")]
    public async Task<ActionResult<DonationSampleResponseDto>> UpdateScreening(Guid sampleId, UpdateScreeningDto dto)
    {
        var result = await _service.UpdateScreeningAsync(sampleId, dto);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{sampleId:guid}/quantity")]
    public async Task<ActionResult<DonationSampleResponseDto>> UpdateQuantity(Guid sampleId, [FromBody] int newQuantity)
    {
        var result = await _service.UpdateQuantityAsync(sampleId, newQuantity);
        if (result is null) return NotFound();
        return Ok(result);
    }
}