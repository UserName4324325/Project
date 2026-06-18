using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBankApplication.DTOs;
using WebBankApplication.Repository;



namespace WebBankApplication.Controllers;

[Authorize]
[ApiController]
[Route("api/remittance")]
public class RemittanceController : ControllerBase
{
    private readonly IRemittanceRepository _repository;

    public RemittanceController(IRemittanceRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddRemittance([FromBody] AddRemittanceDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var result = await _repository.AddRemittanceAsync(dto);
        if (!result) return BadRequest("Ошибка транзакции.");
        return Ok();
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<ResponseRemittanceDto>>> GetHistory()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var history = await _repository.GetRemittanceHistoryAsync(Guid.Parse(userIdClaim.Value));
        return Ok(history);
    }
}