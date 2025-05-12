using FluentValidation;
using Library.Contracts.Requests.Auth;

namespace Library.Application.Infrastructure.Validators.Auth;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Matches(@"^[a-zA-Z0-9 _@.+-]+$") // include space and default valid chars
            .WithMessage("Username contains invalid characters.")
            .MinimumLength(6).WithMessage("Username must be at least 6 characters long.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}