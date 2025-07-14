namespace Movies.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Movies
    {
        private const string MoviesBase = $"{ApiBase}/movies";

        public const string Create = MoviesBase;
        public const string Get = $"{MoviesBase}/{{idOrSlug}}";
        public const string GetAll = MoviesBase;
        public const string Update = $"{MoviesBase}/{{id:guid}}";
        public const string Delete = $"{MoviesBase}/{{id:guid}}";
    }
}
