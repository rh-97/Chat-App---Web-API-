using Test.Helpers;
using Test.Services;

namespace Test.Middlewares;

public class UpdateCacheMiddleware : IMiddleware
{
    private readonly CacheService _cacheService;
    public UpdateCacheMiddleware(CacheService cacheService)
    {
        _cacheService = cacheService;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {

        string auth_header = context.Request.Headers["Authorization"];

        if (auth_header != null && auth_header.StartsWith("Bearer "))
        {
            Console.WriteLine(auth_header);
            string accessToken = auth_header.Substring("Bearer ".Length).Trim();
            var principal = JwtHelpers.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;

            await _cacheService.Set<int>("dr-"+username, 1);
        }
        else Console.WriteLine("No auth header present");





        await next(context);
    }
}
