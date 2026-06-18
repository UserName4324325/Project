using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    public async Task<ResponseDepositDto> AddDeposit(AddDepositDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);

        if (user == null || user.Balance < dto.Amount)
            throw new Exception("Недостаточно средств или пользователь не найден");

        var deposit = new Deposit
        {
            Amount = dto.Amount,
            InterestRate = dto.InterestRate,
            TermInSeconds = dto.TermInSeconds,
            Profit = CalculateProfit(dto.Amount, dto.InterestRate, dto.TermInSeconds),
            UserId = dto.UserId
        };

        user.Balance -= dto.Amount;

        await _context.Deposits.AddAsync(deposit);
        await _context.SaveChangesAsync();

        return MapToResponseDepositDto(deposit);
    }
    private decimal CalculateProfit(decimal amount, float rate, int second) =>
         amount * (decimal)(rate / 100) * second / 12m;
    private ResponseDepositDto MapToResponseDepositDto(Deposit d) =>
        new ResponseDepositDto(d.Id, d.Amount, d.InterestRate, d.TermInSeconds ,d.StartDate, d.Profit, d.IsClosed);

    public async Task<List<ResponseDepositDto>> GetDeposits(Guid userId)
    {
        return await _context.Deposits
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.StartDate)
            .Select(AsResponseDepositDto)
            .ToListAsync();

    }

    private static readonly Expression<Func<Deposit, ResponseDepositDto>> AsResponseDepositDto = l =>
    new ResponseDepositDto
    (
        l.Id, 
        l.Amount,
        l.InterestRate,
        l.TermInSeconds, 
        l.StartDate, 
        l.Profit,
        l.IsClosed
    );


    // background method
    public async Task ProcessExpiredDeposits()
    {
        var expiredDeposits = await _context.Deposits
            .Include(d => d.User)
            .Where(d => !d.IsClosed && d.StartDate.AddSeconds(d.TermInSeconds) <= DateTime.UtcNow)
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
