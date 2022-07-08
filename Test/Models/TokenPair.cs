namespace Test.Models;

public class TokenPair : ITokenPair
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
