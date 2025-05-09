using FluentValidation;
using Library.Contracts.Requests;

namespace Library.Application.Infrastructure.Validators;

public class PagedRequestValidator : AbstractValidator<PagedRequest>
{
    public PagedRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 30)
            .WithMessage("Page size must be between 1 and 30.");
    }
}