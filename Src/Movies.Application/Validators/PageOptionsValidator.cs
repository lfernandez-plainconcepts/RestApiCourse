using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class PageOptionsValidator : AbstractValidator<PageOptions>
{
    public PageOptionsValidator()
    {
        RuleFor(options => options.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");

        RuleFor(options => options.PageSize)
            .InclusiveBetween(1, 25)
            .WithMessage("Page size must be between 1 and 25.");
    }
}
