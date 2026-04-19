using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBankApplication.DTOs;
using WebBankApplication.Repository;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RemittanceController : ControllerBase
{
    private readonly IRemittanceRepository _repository;

    public RemittanceController(IRemittanceRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("remittance")]
    public async Task<IActionResult> RemittanceAdd([FromBody] RemittanceAddDto dto)
    {
        var result = await _repository.RemittanceAddAsync(dto);
        if (!result) return BadRequest("Ошибка транзакции.");
        return Ok();
    }

    [HttpGet("history/{userId}")]
    public async Task<ActionResult<List<RemittanceHistoryDto>>> GetHistory(Guid userId)
    {
        var history = await _repository.GetRemittanceHistoryAsync(userId);
        return Ok(history);
    }
}