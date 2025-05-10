using FluentValidation;
using Library.Contracts.Requests.Book;

namespace Library.Application.Infrastructure.Validators.Book;

public class BooksRequestValidator : AbstractValidator<BooksRequest>
{
    private static readonly string[] AcceptableSortFields =
    [
        "genre", 
        "author"
    ];
    
    public BooksRequestValidator()
    {
        RuleFor(x => x.SortBy)
            .Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage("You can only sort by 'genre' or 'author'.");
        
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 30)
            .WithMessage("Page size must be between 1 and 30.");
    }
}