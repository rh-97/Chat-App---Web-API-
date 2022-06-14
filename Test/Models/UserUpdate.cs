using System.ComponentModel.DataAnnotations;

namespace Test.Models;

public class UserUpdate : IUserUpdate
{
    [Required]
    public string Id { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public DateTime DateOfBirth { get; set; }
}
