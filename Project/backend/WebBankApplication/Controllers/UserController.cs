using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBankApplication.Repository;


namespace WebBankApplication.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepo;

    public UserController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var user = await _userRepo.GetByIdAsync(Guid.Parse(userIdClaim.Value));
        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var users = await _userRepo.GetAllUsersExceptCurrentAsync(Guid.Parse(userIdClaim.Value));
        if (users == null || users.Count == 0) return NotFound();

        return Ok(users);
    }
}
