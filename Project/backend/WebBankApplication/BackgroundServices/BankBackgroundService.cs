using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebBankApplication.Repository;


namespace WebBankApplication.BackgroundServices;


public class BankBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BankBackgroundService> _logger;

    public BankBackgroundService(IServiceProvider services, ILogger<BankBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Фоновая служба банка запущена.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var depositRepo = scope.ServiceProvider.GetRequiredService<IDepositRepository>();
                    var loanRepo = scope.ServiceProvider.GetRequiredService<ILoanRepository>();

                    _logger.LogInformation("Проверка...");
                    await depositRepo.ProcessExpiredDeposits();
                    await loanRepo.ProcessLoanPayments();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в фоновой службе при обработке транзакций.");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}