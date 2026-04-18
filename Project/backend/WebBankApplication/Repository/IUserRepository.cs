using System;
using System.Threading.Tasks;
using WebBankApplication.DTOs;

namespace WebBankApplication.Repository;

public interface IUserRepository
{
    Task<UserResponseDtos?> GetByIdAsync(Guid id);
    Task<decimal> GetBalanceAsync(Guid id);
}
