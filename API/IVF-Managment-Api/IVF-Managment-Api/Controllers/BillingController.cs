using IVF_Managment_Api.Dtos;
using IVF_Managment_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IVF_Managment_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IBillingService _service;

    public BillingController(IBillingService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<BillResponseDto>> Create(CreateBillDto dto)
    {
        try
        {
            var result = await _service.CreateBillAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BillResponseDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("patient/{patientId:guid}")]
    public async Task<ActionResult<IEnumerable<BillResponseDto>>> GetByPatient(Guid patientId)
    {
        var result = await _service.GetByPatientAsync(patientId);
        return Ok(result);
    }

    [HttpPost("line-items")]
    public async Task<ActionResult<BillLineItemResponseDto>> AddLineItem(CreateBillLineItemDto dto)
    {
        try
        {
            var result = await _service.AddLineItemAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("payments")]
    public async Task<ActionResult<PaymentResponseDto>> RecordPayment(CreatePaymentDto dto)
    {
        try
        {
            var result = await _service.RecordPaymentAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}