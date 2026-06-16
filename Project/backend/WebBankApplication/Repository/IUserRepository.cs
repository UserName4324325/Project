using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBankApplication.DTOs;

namespace WebBankApplication.Repository;

public interface IUserRepository
{
    Task<UserResponseDtos?> GetByIdAsync(Guid id);
    Task<List<AllUsersResponseDtos>> GetAllUsersExceptCurrentAsync(Guid CurrentUserId);
    Task<bool> UpdateUserAsync(UserUpdateDtos dto);
}
