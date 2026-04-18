using System;
using System.Threading.Tasks;
using WebBankApplication.DTOs;
using WebBankApplication.Models;

namespace WebBankApplication.Repository;

public interface IAuthRepository
{
    Task<User?> Register(User user, string password);
    Task<UserAuthResponseDto?> Login(string email, string password);
    Task<bool> UserExists(string email);
}
