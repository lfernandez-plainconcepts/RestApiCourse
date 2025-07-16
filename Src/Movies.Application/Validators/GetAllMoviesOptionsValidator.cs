using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;
public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(options => options.YearOfRelease)
            .InclusiveBetween(1888, DateTime.Now.Year)
            .WithMessage($"Year of release must be between 1888 and {DateTime.Now.Year}.");

        RuleFor(options => options.SortBy)
            .Must(sort => sort is null || sort.IsValid())
            .WithMessage($"Sort options are invalid. Valid options are: {string.Join(", ", SortOptions.ValidSortFields)}");

        RuleFor(options => options.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");

        RuleFor(options => options.PageSize)
            .InclusiveBetween(1, 25)
            .WithMessage("Page size must be between 1 and 25.");
    }
}
