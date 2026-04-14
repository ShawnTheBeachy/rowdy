using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Rowdy;

public static class EndpointConventionBuilderExtensions
{
    /// <summary>
    /// Returns 404 if the specified query string value does not exist or does not match.
    /// </summary>
    public static T WhenQuery<T>(
        this T builder,
        string queryKey,
        string? value,
        StringComparison comparison = StringComparison.Ordinal
    )
        where T : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(
            async (ctx, next) =>
            {
                if (ctx.HttpContext.Request.Query[queryKey].ToString().Equals(value, comparison))
                    return await next(ctx);

                return TypedResults.NotFound();
            }
        );
        return builder;
    }
}
