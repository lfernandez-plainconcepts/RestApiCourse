using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class SortOptionsValidator : AbstractValidator<SortOptions>
{
    public SortOptionsValidator()
    {
        RuleFor(options => options.Field)
            .Must(sortField => sortField is null || SortOptions.ValidSortFields.Contains(sortField.ToLowerInvariant()))
            .WithMessage($"Sort options are invalid. Valid options are: {string.Join(", ", SortOptions.ValidSortFields)}");
    }
}
