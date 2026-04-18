using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using WebBankApplication.Data;

namespace WebBankApplication.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var context = services.GetRequiredService<AppDbContext>();

        for (int i = 1; i <= 5; i++)
        {
            try
            {
                logger.LogInformation("Попытка применения миграций {Step}/5...", i);

                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                    logger.LogInformation("Миграции успешно применены.");
                }
                else
                {
                    logger.LogInformation("Новых миграций не обнаружено.");
                }

                return; 
            }
            catch (Exception ex)
            {
                logger.LogWarning("База данных пока недоступна (Попытка {Step}): {Message}", i, ex.Message);

                if (i == 5) 
                {
                    logger.LogCritical(ex, "Не удалось применить миграции после 5 попыток.");
                    throw;
                }

                Thread.Sleep(5000); 
            }
        }
    }
}