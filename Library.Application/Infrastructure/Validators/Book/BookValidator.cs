using FluentValidation;

namespace Library.Application.Infrastructure.Validators.Book;

public class BookValidator : AbstractValidator<Data.Models.Book>
{
    public BookValidator()
    {
        RuleFor(x =>x.Isbn)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(20)
            .WithMessage($"ISBN must be between 10 and 20 characters.");
        
        RuleFor(x =>x.Title)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Title must be less than 50 characters.");

        RuleFor(x =>x.Genre)
            .NotEmpty()
            .MaximumLength(30)
            .WithMessage("Genre must be less than 30 characters.");
        
        RuleFor(x =>x.Description)
            .NotEmpty()
            .MaximumLength(300)
            .WithMessage("Description must be less than 50 characters.");
        
    }
}