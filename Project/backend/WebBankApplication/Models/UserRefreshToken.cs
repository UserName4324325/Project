using System;

namespace WebBankApplication.Models;

public class UserRefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();


    public Guid UserId { get; set; }
    public User User { get; set; } = null!;


    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}