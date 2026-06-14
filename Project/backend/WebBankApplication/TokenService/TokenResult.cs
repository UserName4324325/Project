using System;

namespace WebBankApplication.TokenService;

public class TokenResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
