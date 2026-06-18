using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebBankApplication.DTOs;
using WebBankApplication.Models;
using WebBankApplication.Repository;
using WebBankApplication.TokenService;


namespace WebBankApplication.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepo;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthRepository authRepo, ITokenService tokenService)
    {
        _authRepo = authRepo;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto request)
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
    public async Task<IActionResult> Login([FromBody] UserLoginDto request)
    {
        var authResponse = await _authRepo.Login(request.Email, request.Password);

        if (authResponse == null)
            return Unauthorized(new { message = "Неверный Email или пароль" });


        SetRefreshTokenCookie(authResponse.RefreshToken, DateTime.UtcNow.AddDays(7));


        return Ok( new
        {
            id = authResponse.Id,
            token = authResponse.Token,
            fullName = authResponse.FullName,
            balance = authResponse.Balance
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var oldRefreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(oldRefreshToken))
            return Unauthorized(new { message = "Сессия отсутсвует или истекла"});

        var tokenRecord = await _authRepo.GetRefreshTokenAsync(oldRefreshToken);

        if (tokenRecord == null || tokenRecord.ExpiryTime < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Невалидный или просроченный токен обновления. Войдите зановою" });
        }


        await _authRepo.DeleteRefreshTokenAsync(oldRefreshToken);

        var newTokens = _tokenService.CreateToken(tokenRecord.User);

        await _authRepo.SaveRefreshTokenAsync(tokenRecord.UserId, newTokens.RefreshToken, newTokens.RefreshTokenExpiryTime);

        SetRefreshTokenCookie(newTokens.RefreshToken, newTokens.RefreshTokenExpiryTime);


        return Ok(new { token = newTokens.AccessToken });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authRepo.DeleteRefreshTokenAsync(refreshToken);
        }

        Response.Cookies.Delete("refreshToken");

        return Ok(new { message = "Вы успешно вышли из системы" });
    }

    private void SetRefreshTokenCookie(string token, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expires
        };

        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}
