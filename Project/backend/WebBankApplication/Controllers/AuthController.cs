using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebBankApplication.DTOs;
using WebBankApplication.Models;
using WebBankApplication.Repository;


namespace WebBankApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepo;

    public AuthController(IAuthRepository authRepo)
    {
        _authRepo = authRepo;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto request)
    {
        if (await _authRepo.UserExists(request.Email))
            return BadRequest(new { message = "Пользователь с таким Email уже зарегистрирован" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email
        };

        var createdUser = await _authRepo.Register(user, request.Password);

        if (createdUser == null)
            return StatusCode(500, "Ошибка при создании пользователя");

        return Ok(new { message = "Регистрация прошла успешно" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto request)
    {
        var authResponse = await _authRepo.Login(request.Email, request.Password);

        if (authResponse == null)
            return Unauthorized(new { message = "Неверный Email или пароль" });

        return Ok(authResponse);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var user = await _authRepo.GetUserById(Guid.Parse(userIdClaim.Value));
        if (user == null) return NotFound();

        return Ok( new AuthResponseDto (

            Token: "",
            Id: user.Id,
            FullName: user.FullName,
            Balance: user.Balance

        ));
    }
}
