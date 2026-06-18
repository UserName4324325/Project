using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBankApplication.DTOs;
using WebBankApplication.Repository;

namespace WebBankApplication.Controllers;


[ApiController]
[Route("api/deposit")]
[Authorize]
public class DepositController : ControllerBase
{
    private readonly IDepositRepository _depositRepo;

    public DepositController(IDepositRepository depositRepo)
    {
        _depositRepo = depositRepo;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddDeposit([FromBody] AddDepositDto dto)
    {
        try
        {
            var result = await _depositRepo.AddDeposit(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetDeposits()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var deposits = await _depositRepo.GetDeposits(Guid.Parse(userIdClaim.Value));
        return Ok(deposits);
    }
}
