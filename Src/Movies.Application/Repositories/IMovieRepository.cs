﻿using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(
        Movie movie,
        Guid? userId = default,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Movie>> GetAllAsync(
        MoviesFilterOptions filterOptions,
        PageOptions pageOptions,
        SortOptions? sortOptions = default,
        CancellationToken cancellationToken = default);

    Task<Movie?> GetByIdAsync(
        Guid id,
        Guid? userId = default,
        CancellationToken cancellationToken = default);

    Task<Movie?> GetBySlugAsync(
        string slug,
        Guid? userId = default,
        CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(
        MoviesFilterOptions options,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Movie movie,
        CancellationToken cancellationToken = default);
}
