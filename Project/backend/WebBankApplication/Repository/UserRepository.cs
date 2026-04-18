using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBankApplication.Data;
using WebBankApplication.DTOs;
using WebBankApplication.Models;

namespace WebBankApplication.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetBalanceAsync(Guid id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Select(u => u.Balance)
            .FirstOrDefaultAsync();
    }

    public async Task<UserResponseDtos?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        return MapToUserResponseDto(user);
    }
    private UserResponseDtos MapToUserResponseDto(User user) =>
        new UserResponseDtos(user.Id, user.FullName, user.Email, user.Balance);

}
