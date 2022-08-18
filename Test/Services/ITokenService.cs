using System.Security.Claims;
using Test.Models;

namespace Test.Services;

public interface ITokenService
{
    Task<bool> VerifyLogin(UserLogin user);
    Task<bool> SaveRefreshToken(Token refreshToken);
    Task<bool> DaleteRefreshToken(string name, string refreshToken);
    Task<Token> GetToken(string name, string refreshToken);
}
