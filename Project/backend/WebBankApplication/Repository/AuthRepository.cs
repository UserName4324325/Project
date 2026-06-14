using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebBankApplication.Data;
using WebBankApplication.DTOs;
using WebBankApplication.Models;
using WebBankApplication.TokenService;

namespace WebBankApplication.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    private const decimal InitialUserBalance = 100_000m;

    public AuthRepository(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<User?> Register(User user, string password)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        user.Balance = InitialUserBalance;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<UserAuthResponseDto?> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        var tokenResult = _tokenService.CreateToken(user);

        await SaveRefreshTokenAsync(user.Id, tokenResult.RefreshToken, tokenResult.RefreshTokenExpiryTime);


        return new UserAuthResponseDto
        (
            Id: user.Id,
            Token: tokenResult.AccessToken,
            RefreshToken: tokenResult.RefreshToken,
            FullName: user.FullName,
            Balance: user.Balance
        );
    }

    public async Task<UserRefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.UserRefreshTokens
            .Include(u => u.User)
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task DeleteRefreshTokenAsync(string token)
    {
        var tokenRefresh = await _context.UserRefreshTokens.FirstOrDefaultAsync(t => t.Token == token);

        if (tokenRefresh != null)
        {
            _context.UserRefreshTokens.Remove(tokenRefresh);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveRefreshTokenAsync(Guid userId, string token, DateTime expiryTime)
    {
        var refreshToken = new UserRefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiryTime = expiryTime
        };

        await _context.UserRefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UserExists(string email) =>
        await _context.Users.AnyAsync(u => u.Email == email);
}
