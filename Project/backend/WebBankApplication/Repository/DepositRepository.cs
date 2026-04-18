using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBankApplication.Data;
using WebBankApplication.DTOs;
using WebBankApplication.Models;

namespace WebBankApplication.Repository;

public class DepositRepository : IDepositRepository
{
    private readonly AppDbContext _context;

    public DepositRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DepositResponseDto> OpenDeposit(OpenDepositDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);

        if (user == null || user.Balance < dto.Amount)
            throw new Exception("Недостаточно средств или пользователь не найден");

        decimal profit = CalculateProfit(dto.Amount, dto.InterestRate, dto.TermInSeconds);

        var deposit = new Deposit
        {
            Amount = dto.Amount,
            InterestRate = dto.InterestRate,
            TermInSeconds = dto.TermInSeconds,
            Profit = profit,

            UserId = dto.UserId
        };

        user.Balance -= dto.Amount;

        await _context.Deposits.AddAsync(deposit);
        await _context.SaveChangesAsync();

        return MapToResponseDto(deposit);
    }

    public async Task<List<DepositResponseDto>> GetUserDeposits(Guid userId)
    {
        var deposits = await _context.Deposits
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.StartDate)
            .ToListAsync();

        return deposits.Select(MapToResponseDto).ToList();
    }

    private decimal CalculateProfit(decimal amount, float rate, int second) =>
         amount * (decimal)(rate / 100) * second / 12m;

    private DepositResponseDto MapToResponseDto(Deposit d) =>
        new DepositResponseDto(d.Id, d.Amount, d.InterestRate, d.TermInSeconds ,d.StartDate, d.Profit, d.IsClosed);


    // background method
    public async Task ProcessExpiredDeposits()
    {
        var now = DateTime.UtcNow;

        var expiredDeposits = await _context.Deposits
            .Include(d => d.User)
            .Where(d => !d.IsClosed && d.StartDate.AddSeconds(d.TermInSeconds) <= now)
            .ToListAsync();

        foreach (var dep in expiredDeposits)
        {
            if (dep.User != null)
            {
                dep.User.Balance += dep.Amount + dep.Profit;
                dep.IsClosed = true;
            }
        }

        await _context.SaveChangesAsync();
    }
}
