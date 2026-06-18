using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBankApplication.DTOs;

namespace WebBankApplication.Repository;

public interface IUserRepository
{
    Task<UserResponseDtos?> GetByIdAsync(Guid id);
    Task<List<UsersResponseDtos>> GetAllUsersAsync();
    Task<List<UsersResponseDtos>> SearchUsersAsync(string query, Guid currentUserId);
}
