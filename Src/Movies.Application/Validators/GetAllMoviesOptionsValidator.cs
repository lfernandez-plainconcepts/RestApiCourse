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
    }
}
