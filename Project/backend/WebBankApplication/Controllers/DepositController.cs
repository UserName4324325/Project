using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebBankApplication.DTOs;
using WebBankApplication.Repository;

namespace WebBankApplication.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepositsController : ControllerBase
{
    private readonly IDepositRepository _depositRepo;

    public DepositsController(IDepositRepository depositRepo)
    {
        _depositRepo = depositRepo;
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenDeposit([FromBody] OpenDepositDto dto)
    {
        try
        {
            var result = await _depositRepo.OpenDeposit(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserDeposits(Guid userId)
    {
        var deposits = await _depositRepo.GetUserDeposits(userId);
        return Ok(deposits);
    }
}
