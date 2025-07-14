﻿using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }
    public string Slug => GenerateSlug();
    public required string Title { get; set; }
    public required int YearOfRelease { get; set; }
    public required List<string> Genres { get; set; } = [];

    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex()
            .Replace(Title, string.Empty)
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Trim();

        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}
