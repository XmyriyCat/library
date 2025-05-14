using FluentValidation;
using Library.Contracts.Requests.Book;

namespace Library.Application.Infrastructure.Validators.Book;

public class BooksRequestValidator : AbstractValidator<BooksRequest>
{
    public BooksRequestValidator()
    {
        RuleFor(x => x.Title)
            .Must(x => x is null || x.Length < 50 )
            .WithMessage("You can only search by 'title' with max length 50 characters.");
        
        RuleFor(x => x.Genre)
            .Must(x => x is null || x.Length < 30 )
            .WithMessage("You can only filter by 'genre' with max length 30 characters.");
        
        RuleFor(x => x.Author)
            .Must(x => x is null || x.Length < 50 )
            .WithMessage("You can only filter by 'author' with max length 50 characters.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 30)
            .WithMessage("Page size must be between 1 and 30.");
    }
}