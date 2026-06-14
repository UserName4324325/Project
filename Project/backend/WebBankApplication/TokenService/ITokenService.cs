using WebBankApplication.Models;

namespace WebBankApplication.TokenService;

public interface ITokenService
{
    TokenResult CreateToken(User user);
}
