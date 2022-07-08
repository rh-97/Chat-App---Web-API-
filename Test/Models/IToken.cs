namespace Test.Models;

public interface IToken
{
    string? Id { get; set;  }
    string Name { get; set; }
    string RefreshToken { get; set; }
    DateTime ExpireDateTime { get; set; }
}
