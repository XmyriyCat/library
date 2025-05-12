using FluentValidation;
using Library.Contracts.Requests.UserBook;

namespace Library.Application.Infrastructure.Validators.UserBook;

public class BorrowedBooksRequestValidator : AbstractValidator<BorrowedBooksRequest>
{
    public BorrowedBooksRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
            .LessThanOrEqualTo(30).WithMessage("PageSize cannot exceed 30.");
    }
}