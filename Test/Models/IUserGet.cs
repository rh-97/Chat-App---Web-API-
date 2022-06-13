namespace Test.Models;

public interface IUserGet
{
    string Id { get; set; }
    string Name { get; set; }
    string Email { get; set; }
    DateTime DateOfBirth { get; set; }
}
