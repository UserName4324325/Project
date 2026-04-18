using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBankApplication.Data;
using WebBankApplication.DTOs;
using WebBankApplication.Models;

namespace WebBankApplication.Repository;

public class LoanRepository : ILoanRepository
{
    private readonly AppDbContext _context;

    public LoanRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LoanResponseDto> TakeLoan(TakeLoanDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);

        if (user == null) throw new Exception("Пользователь не найден");

        // АННУИТЕТНАЯ МАТЕМАТИКА
        decimal monthlyRate = (decimal)dto.InterestRate / 12 / 100;
        decimal pow = (decimal)Math.Pow(1 + (double)monthlyRate, dto.TermInSeconds);

        decimal paymentPerSecond = dto.Amount * (monthlyRate * pow) / (pow - 1);

        decimal totalToReturn = paymentPerSecond * dto.TermInSeconds;
        //

        var loan = new Loan
        {
            TotalAmount = totalToReturn,
            RemainingAmount = totalToReturn,
            InterestRate = dto.InterestRate,
            TermInSeconds = dto.TermInSeconds,
            PerSecondPayment = paymentPerSecond,

            UserId = dto.UserId
        };

        user.Balance += dto.Amount;

        await _context.Loans.AddAsync(loan);
        await _context.SaveChangesAsync();

        return MapToLoanResponseDto(loan);
    }

    public async Task<List<LoanResponseDto>> GetUserLoans(Guid userId)
    {
        var loans = await _context.Loans
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.StartDate)
            .ToListAsync();

        return loans.Select(MapToLoanResponseDto).ToList();
    }

    // background method
    public async Task ProcessLoanPayments()
    {
        var activeLoans = await _context.Loans
            .Include(l => l.User)
            .Where(l => !l.IsPaid)
            .ToListAsync();

        foreach (var loan in activeLoans)
        {
            if (loan.User == null) continue;

            decimal payment = Math.Min(loan.RemainingAmount, loan.PerSecondPayment);

            loan.User.Balance -= payment;
            loan.RemainingAmount -= payment;

            if (loan.RemainingAmount <= 0.01m)
            {
                loan.RemainingAmount = 0;
                loan.IsPaid = true;
            }
        }

        await _context.SaveChangesAsync();
    }

    private LoanResponseDto MapToLoanResponseDto(Loan l) =>
        new LoanResponseDto(l.Id, l.TermInSeconds, l.TotalAmount, l.RemainingAmount, l.InterestRate, l.StartDate, l.PerSecondPayment, l.IsPaid);
}