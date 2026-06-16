using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBankApplication.DTOs;
using WebBankApplication.Repository;
using WebBankApplication.TokenService;


namespace WebBankApplication.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;

    public UserController(IUserRepository userRepo, ITokenService tokenService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
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

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDtos dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null) return Unauthorized();
        if (dto.Id != Guid.Parse(userIdClaim.Value)) return Forbid();


        var user = await _userRepo.UpdateUserAsync(dto);
        if (!user) return BadRequest("Не удалось обновить профиль. Проверьте пароли или данные.");

        var updatedUser = await _userRepo.GetByIdAsync(dto.Id);
        var newToken = _tokenService.CreateToken(updatedUser);

        return Ok(new { message = "Профиль успешно обновлен", user = updatedUser });
    }
}
