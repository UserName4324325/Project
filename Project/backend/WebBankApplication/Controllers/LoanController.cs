using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBankApplication.DTOs;
using WebBankApplication.Repository;


namespace WebBankApplication.Controllers;

[ApiController]
[Route("api/loan")]
[Authorize]
public class LoanController : ControllerBase
{
    private readonly ILoanRepository _loanRepo;

    public LoanController(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddLoan([FromBody] AddLoanDto dto)
    {
        try
        {
            var result = await _loanRepo.AddLoan(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetLoans()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var loans = await _loanRepo.GetLoans(Guid.Parse(userIdClaim.Value));
        return Ok(loans);
    }
}