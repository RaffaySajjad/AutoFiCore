namespace AutoFiCore.Middleware
{
    public static class TokenValidatorExtensions
    {
        public static IApplicationBuilder UseTokenValidatorLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidator>();
        }
    }
}
