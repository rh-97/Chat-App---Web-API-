namespace Test.Models
{
    public interface IUser
    {
        string? Id { get; set; }
        string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        string Password { get; set; }

    }
}