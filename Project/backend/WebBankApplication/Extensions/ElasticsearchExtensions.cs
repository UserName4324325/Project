using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebBankApplication.Data;

namespace WebBankApplication.Extensions;

public static class ElasticsearchExtensions
{
    public static async Task SeedElasticsearchDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILogger<Program>>();
        
        try
        {
            var dbContext = services.GetRequiredService<AppDbContext>();
            var client = services.GetRequiredService<ElasticsearchClient>();

            var indexExistsResponse = await client.Indices.ExistsAsync("users");

            if (!indexExistsResponse.Exists)
            {
                await client.Indices.CreateAsync("users");

                var allUsers = await dbContext.Users.AsNoTracking().ToListAsync();

                if (allUsers.Any())
                {
                    var documentsToIndex = allUsers.Select(user => new
                    {
                        user.Id,
                        user.FullName,
                        user.Email
                    }).ToList();

                    var bulkResponse = await client.IndexManyAsync(documentsToIndex, "users");

                    if (!bulkResponse.IsValidResponse)
                    {
                        logger.LogError($"[Elasticsearch] Ошибка массовой индексации: {bulkResponse.DebugInformation}");
                    }
                    else
                    {
                        logger.LogInformation($"[Elasticsearch] Успешно перенесено {documentsToIndex.Count} пользователей при первом запуске.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"[Elasticsearch] Не удалось выполнить первичную синхронизацию: {ex.Message}");
        }
    }
}