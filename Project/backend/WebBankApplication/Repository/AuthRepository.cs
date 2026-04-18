using Microsoft.EntityFrameworkCore;
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

        var token = _tokenService.CreateToken(user);

        return new UserAuthResponseDto
        (
            Id: user.Id,
            Token: token,
            FullName: user.FullName,
            Balance: user.Balance
        );
    }

    public async Task<bool> UserExists(string email) =>
        await _context.Users.AnyAsync(u => u.Email == email);
}
