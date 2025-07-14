using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;

    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;

        RuleFor(movie => movie.Id)
            .NotEmpty()
            .WithMessage("Id is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("Id must not be an empty GUID.");

        RuleFor(movie => movie.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(movie => movie.Genres)
            .NotEmpty()
            .WithMessage("At least one genre is required.");

        RuleFor(movie => movie.YearOfRelease)
            .InclusiveBetween(1888, DateTime.Now.Year)
            .WithMessage($"Year of release must be between 1888 and {DateTime.Now.Year}.");

        RuleFor(movie => movie.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exists in the system.");
    }

    private async Task<bool> ValidateSlug(Movie movie,
        string slug,
        CancellationToken cancellationToken)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug);

        if (existingMovie is not null)
        {
            return existingMovie.Id == movie.Id;
        }

        return existingMovie is null;
    }
}
