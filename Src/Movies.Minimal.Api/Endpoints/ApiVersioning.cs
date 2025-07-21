using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;

namespace Movies.Minimal.Api.Endpoints;

public static class ApiVersioning
{
    public static ApiVersionSet VersionSet { get; private set; }

    public static IEndpointRouteBuilder CreateApiVersionSet(this IEndpointRouteBuilder builder)
    {
        VersionSet = builder
            .NewApiVersionSet()
            .HasApiVersion(1.0)
            .HasApiVersion(2.0)
            .ReportApiVersions()
            .Build();

        return builder;
    }
}
