namespace Test.Models
{
    public interface IUserRegister
    {
        string Name { get; set; }

        string Email { get; set; }

        DateTime DateOfBirth { get; set; }

        string Password { get; set; }

        string ConfirmPassword { get; set; }
    }
}
