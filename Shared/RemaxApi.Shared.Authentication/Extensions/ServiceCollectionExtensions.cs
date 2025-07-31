using Microsoft.Extensions.DependencyInjection;
using RemaxApi.Shared.Authentication.Services;

namespace RemaxApi.Shared.Authentication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra i servizi necessari per l'autenticazione JWT esterna
        /// Nota: HttpContextAccessor deve essere già registrato nel progetto host
        /// </summary>
        /// <param name="services">La collection dei servizi</param>
        /// <returns>La collection dei servizi per il chaining</returns>
        public static IServiceCollection AddExternalJwtAuthentication(this IServiceCollection services)
        {
            // Registra il servizio di autenticazione
            services.AddScoped<IExternalAuthUserService, ExternalAuthUserService>();
            
            return services;
        }
    }
}