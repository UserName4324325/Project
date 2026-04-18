using Microsoft.AspNetCore.Mvc;
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
}
