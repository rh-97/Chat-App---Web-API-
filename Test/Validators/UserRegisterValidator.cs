using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using Test.Database;
using Test.Models;
using Test.Services;

namespace Test.Validators;

public class UserRegisterValidator : AbstractValidator<UserRegister>
{

    public UserRegisterValidator(UserService service)
    {

        // Username starts with a letter and
        // has minimum length 4 and
        // contains only letters and numbers
        RuleFor(u => u.Name)
            .MinimumLength(4).MaximumLength(30).Matches(new Regex("^[A-Za-z][(A-Z)(a-z)(0-9)-_]*"))
            .WithMessage("Invalid Name");


        // Username must be unique
        RuleFor(u => u.Name)
            .Must(name => service.IsUniqueUsername(name).Result)
            .WithMessage("Username already exists.");



        // Email is a valid email address
        RuleFor(u => u.Email)
            .EmailAddress()
            .WithMessage("Invalid Email");


        // Email must be unique
        RuleFor(u => u.Email)
            .Must(email => service.IsUniqueEmail(email).Result)
            .WithMessage("Email already exists.");



        // Age is at least 18 years
        RuleFor(u => u.DateOfBirth)
            .Must(d => d.AddYears(18) <= DateTime.Now)
            .WithMessage("Age must be greater than 18.");


        // Password has at least one uppercase letter,
        // one lowercase letter,
        // one number,
        // one special character, and
        // minimum length is 8
        RuleFor(u => u.Password)
            .Matches(new Regex("(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9])(?=.{8,})"))
            .WithMessage("password must be of minimum 8 characters " +
            "containing at least one lowercase, one upprecase, one number, and one special character.");


        // Password and ConfirmPassword matches
        RuleFor(u => u)
            .Must(u => u.Password == u.ConfirmPassword)
            .WithName("ConfirmPassword").WithMessage("Password confirmation failed");
    }
    


}
