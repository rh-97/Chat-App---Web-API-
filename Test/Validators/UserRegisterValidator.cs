using FluentValidation;
using System.Text.RegularExpressions;
using Test.Models;

namespace Test.Validators;

public class UserRegisterValidator : AbstractValidator<UserRegister>
{
    public UserRegisterValidator()
    {
        RuleFor(u => u.Name)
            .MinimumLength(4).MaximumLength(30).Matches(new Regex("^[A-Za-z][(A-Z)(a-z)(0-9)-_]*"))
            .WithName("NameError").WithMessage("Invalid Name");

        RuleFor(u => u.Email)
            .EmailAddress()
            .WithName("EmailError").WithMessage("Invalid Email");

        RuleFor(u => u.DateOfBirth)
            .Must(d => d.AddYears(18) <= DateTime.Now)
            .WithName("DateError").WithMessage("Age must be greater than 18.");

        RuleFor(u => u)
            .Must(u => u.Password == u.ConfirmPassword)
            .WithName("PasswordError").WithMessage("Password confirmation failed");
    }
    


}
