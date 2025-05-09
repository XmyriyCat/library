using FluentValidation;

namespace Library.Application.Infrastructure.Validators.Author;

public class AuthorValidator : AbstractValidator<Data.Models.Author>
{
    public AuthorValidator()
    {
        RuleFor(x =>x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage($"Name must be less than 50 characters.");
        
        RuleFor(x =>x.Country)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Country must be less than 50 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty();
    }
}