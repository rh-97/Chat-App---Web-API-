using FluentValidation;
using Test.Models;
using Test.Services;

namespace Test.Validators;

public class UserUpdateValidator : AbstractValidator<UserUpdate>
{
    public UserUpdateValidator(UserService service)
    {


        // Email is a valid email address
        RuleFor(u => u.Email)
            .EmailAddress()
            .WithMessage("Invalid Email");


        // Email must be unique
        RuleFor(u => u)
            .Must(u => service.IsUniqueEmailForUpdate(u).Result)
            .WithName("Email")
            .WithMessage("Update Email already exists.");



        // Age is at least 18 years
        RuleFor(u => u.DateOfBirth)
            .Must(d => d.AddYears(18) <= DateTime.Now)
            .WithMessage("Age must be greater than 18.");


    }
}
