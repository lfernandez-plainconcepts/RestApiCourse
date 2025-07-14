using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository movieRepository, IValidator<Movie> validator) : IMovieService
{
    private readonly IMovieRepository _movieRepository = movieRepository;
    private readonly IValidator<Movie> _validator = validator;

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(movie, cancellationToken);
        return await _movieRepository.CreateAsync(movie);
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _movieRepository.DeleteByIdAsync(id, cancellationToken);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetAllAsync(cancellationToken);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetBySlugAsync(slug, cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(movie, cancellationToken);
        var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, cancellationToken);
        if (!movieExists)
        {
            return null;
        }

        await _movieRepository.UpdateAsync(movie, cancellationToken);
        return movie;
    }
}
