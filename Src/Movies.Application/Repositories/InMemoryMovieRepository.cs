﻿using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class InMemoryMovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = [];

    public Task<bool> CreateAsync(
        Movie movie,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var removedCount = _movies.RemoveAll(m => m.Id == id);
        var movieRemoved = removedCount > 0;
        return Task.FromResult(movieRemoved);
    }

    public Task<bool> ExistsByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var exists = _movies.Exists(m => m.Id == id);

        return Task.FromResult(exists);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(
        GetAllMoviesOptions options,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<Movie?> GetByIdAsync(
        Guid id,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var movie = _movies.SingleOrDefault(m => m.Id == id);
        return Task.FromResult(movie);
    }

    public Task<Movie?> GetBySlugAsync(
        string slug,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var movie = _movies.SingleOrDefault(m => m.Slug == slug);
        return Task.FromResult(movie);
    }

    public Task<bool> UpdateAsync(
        Movie movie,
        CancellationToken cancellationToken = default)
    {
        var movieIndex = _movies.FindIndex(m => m.Id == movie.Id);
        if (movieIndex == -1)
            return Task.FromResult(false);

        _movies[movieIndex] = movie;
        return Task.FromResult(true);
    }
}
