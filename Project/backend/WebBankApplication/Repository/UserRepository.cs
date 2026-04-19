using Elastic.Clients.Elasticsearch;
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
        var users = await _context.Users.Where(u => u.Id != CurrentUserId).ToListAsync();
        return users.Select(MapToAllUsersResponseDto).ToList()!;
    }

    private AllUsersResponseDtos? MapToAllUsersResponseDto(User? user) =>
        user == null ? null : new AllUsersResponseDtos(user.Id, user.FullName, user.Email);
    private UserResponseDtos? MapToUserResponseDto(User? user) =>
        user == null ? null : new UserResponseDtos(user.Id, user.FullName, user.Email, user.Balance);

}
