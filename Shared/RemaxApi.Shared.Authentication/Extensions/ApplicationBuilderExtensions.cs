using Microsoft.AspNetCore.Builder;
using RemaxApi.Shared.Authentication.Middleware;

namespace RemaxApi.Shared.Authentication.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Aggiunge il middleware di validazione JWT alla pipeline delle richieste
        /// </summary>
        /// <param name="builder">L'application builder</param>
        /// <returns>L'application builder per il chaining</returns>
        public static IApplicationBuilder UseExternalJwtValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtValidationMiddleware>();
        }
    }
}