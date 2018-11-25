using Microsoft.AspNetCore.Builder;

namespace Eventures.Web.Middleware
{
    public static class SeedRolesMiddlewareExtensions
    {
        public static IApplicationBuilder UseSeedRoles(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SeedRolesMiddleware>();
        }
    }
}
