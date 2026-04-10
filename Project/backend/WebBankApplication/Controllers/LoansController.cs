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
public class LoansController : ControllerBase
{
    private readonly ILoanRepository _loanRepo;

    public LoansController(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    [HttpPost("take")]
    public async Task<IActionResult> TakeLoan([FromBody] TakeLoanDto dto)
    {
        try
        {
            var result = await _loanRepo.TakeLoan(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserLoans(Guid userId)
    {
        var loans = await _loanRepo.GetUserLoans(userId);
        return Ok(loans);
    }
}