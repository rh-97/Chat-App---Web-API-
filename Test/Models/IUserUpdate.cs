namespace Test.Models;

public interface IUserUpdate
{
    string Id { get; set; }
    string Email { get; set; }
    DateTime DateOfBirth { get; set; }
}
