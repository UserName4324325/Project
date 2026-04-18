using WebBankApplication.Models;

namespace WebBankApplication.TokenService;

public interface ITokenService
{
    string CreateToken(User user);
}
