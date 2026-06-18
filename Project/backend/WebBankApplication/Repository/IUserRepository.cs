using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBankApplication.DTOs;

namespace WebBankApplication.Repository;

public interface IUserRepository
{
    Task<UserResponseDtos?> GetByIdAsync(Guid id);
    Task<List<AllUsersResponseDtos>> GetAllUsersAsync(Guid CurrentUserId);
    Task<List<AllUsersResponseDtos>> SearchUsersAsync(string query, Guid currentUserId);
}
