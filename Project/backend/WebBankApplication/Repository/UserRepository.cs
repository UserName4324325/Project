using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

    public async Task<UserResponseDtos?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        return MapToUserResponseDto(user);
    }   
    
    public async Task<List<AllUsersResponseDtos>> GetAllUsersExceptCurrentAsync(Guid CurrentUserId)
    {
        var users = await _context.Users.ToListAsync();
        return users.Select(MapToAllUsersResponseDto).ToList()!;
    }

    public async Task<bool> UpdateUserAsync(UserUpdateDtos dto)
    {
        var user = await _context.Users.FindAsync(dto.Id);
        if (user == null) return false;

        if (!string.IsNullOrEmpty(dto.NewPassword))
        {
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        }

        user.FullName = dto.FullName;
        user.Email = dto.Email;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    private AllUsersResponseDtos? MapToAllUsersResponseDto(User? user) =>
        user == null ? null : new AllUsersResponseDtos(user.Id, user.FullName, user.Email);
    private UserResponseDtos? MapToUserResponseDto(User? user) =>
        user == null ? null : new UserResponseDtos(user.Id, user.FullName, user.Email, user.Balance);

}
