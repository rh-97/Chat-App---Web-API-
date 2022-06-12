using System.ComponentModel.DataAnnotations;

namespace Test.Models;

public class UserLogin : IUserLogin
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
