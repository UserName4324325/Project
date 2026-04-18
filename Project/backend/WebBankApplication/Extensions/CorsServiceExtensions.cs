using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebBankApplication.Extensions;

public static class CorsServiceExtensions
{
    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
            {
                var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();

                policy.WithOrigins(origins ?? new[] { "http://localhost:5173" })
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }
}
