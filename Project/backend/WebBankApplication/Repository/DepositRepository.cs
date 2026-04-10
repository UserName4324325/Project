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
    private readonly ApplicationDbContext _context;

    public DepositRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DepositResponseDto> OpenDeposit(OpenDepositDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);

        if (user == null || user.Balance < dto.Amount)
            throw new Exception("Недостаточно средств или пользователь не найден");

        decimal profit = dto.Amount * (decimal)(dto.InterestRate / 100);

        var deposit = new Deposit
        {
            Id = Guid.NewGuid(),
            Amount = dto.Amount,
            InterestRate = dto.InterestRate,
            StartDate = DateTime.UtcNow,
            IsClosed = false,
            TermInSeconds = dto.TermInSeconds,
            Profit = profit,

            UserId = dto.UserId
        };

        user.Balance -= dto.Amount;

        await _context.Deposits.AddAsync(deposit);
        await _context.SaveChangesAsync();

        return MapToDto(deposit);
    }

    public async Task<List<DepositResponseDto>> GetUserDeposits(Guid userId)
    {
        var deposits = await _context.Deposits
            .Where(d => d.UserId == userId)
            .ToListAsync();

        return deposits.Select(MapToDto).ToList();
    }

    private DepositResponseDto MapToDto(Deposit d) =>
        new DepositResponseDto(d.Id, d.Amount, d.InterestRate, d.TermInSeconds, d.StartDate, d.Profit, d.IsClosed);

    public async Task ProcessExpiredDeposits()
    {
        var now = DateTime.UtcNow;

        var expired = await _context.Deposits
            .Include(d => d.User)
            .Where(d => !d.IsClosed && d.StartDate.AddSeconds(d.TermInSeconds) <= now)
            .ToListAsync();

        foreach (var dep in expired)
        {
            if (dep.User != null)
            {
                decimal profit = dep.Amount * (decimal)(dep.InterestRate / 100);
                dep.User.Balance += (dep.Amount + profit);
                dep.IsClosed = true;
            }
        }

        await _context.SaveChangesAsync();
    }
}
