using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Rowdy;

/// <summary>
/// Extension methods to extend <c>IEndpointConventionBuilder</c> functionality.
/// </summary>
public static class EndpointConventionBuilderExtensions
{
    /// <summary>
    /// Returns 404 if the specified header does not match.
    /// </summary>
    public static T WithHeaderFilter<T>(
        this T builder,
        string headerName,
        string? headerValue,
        StringComparison comparison = StringComparison.Ordinal
    )
        where T : IEndpointConventionBuilder =>
        builder.WithHeaderFilter(
            headerName,
            x =>
                x == StringValues.Empty
                    ? string.IsNullOrEmpty(headerValue)
                    : string.Equals(headerValue, x.ToString(), comparison)
        );

    /// <summary>
    /// Returns 404 if the specified header does not match.
    /// </summary>
    public static T WithHeaderFilter<T>(
        this T builder,
        string headerName,
        Func<StringValues, bool> match
    )
        where T : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(
            async (ctx, next) =>
            {
                if (match(ctx.HttpContext.Request.Headers[headerName]))
                    return await next(ctx);

                return TypedResults.NotFound();
            }
        );
        return builder;
    }

    /// <summary>
    /// Returns 404 if the specified query string value does not exist or does not match.
    /// </summary>
    public static T WithQueryFilter<T>(
        this T builder,
        string queryKey,
        string? value,
        StringComparison comparison = StringComparison.Ordinal
    )
        where T : IEndpointConventionBuilder =>
        builder.WithQueryFilter(queryKey, x => x.Equals(value, comparison));

    /// <summary>
    /// Returns 404 if the specified query string value does not exist or does not match.
    /// </summary>
    public static T WithQueryFilter<T>(this T builder, string queryKey, Func<string, bool> match)
        where T : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(
            async (ctx, next) =>
            {
                if (match(ctx.HttpContext.Request.Query[queryKey].ToString()))
                    return await next(ctx);

                return TypedResults.NotFound();
            }
        );
        return builder;
    }
}
