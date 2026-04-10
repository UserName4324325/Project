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
    private readonly ApplicationDbContext _context;

    public LoanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LoanResponseDto> TakeLoan(TakeLoanDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);

        if (user == null) throw new Exception("Пользователь не найден");

        // АННУИТЕТНАЯ МАТЕМАТИКА

        double monthlyRate = dto.InterestRate / 12 / 100;
        double pow = Math.Pow(1 + monthlyRate, dto.TermInSeconds);

        decimal paymentPerSecond = (decimal)((double)dto.Amount * (monthlyRate * pow) / (pow - 1));

        decimal totalToReturn = paymentPerSecond * dto.TermInSeconds;

        //

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            TotalAmount = totalToReturn,
            RemainingAmount = totalToReturn,
            InterestRate = dto.InterestRate,
            TermInSeconds = dto.TermInSeconds,
            PerSecondPayment = paymentPerSecond,
            StartDate = DateTime.UtcNow,
            IsPaid = false,
            UserId = dto.UserId
        };

        user.Balance += dto.Amount;

        await _context.Loans.AddAsync(loan);
        await _context.SaveChangesAsync();

        return MapToDto(loan);
    }

    public async Task<List<LoanResponseDto>> GetUserLoans(Guid userId)
    {
        var loans = await _context.Loans
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.StartDate)
            .ToListAsync();

        return loans.Select(MapToDto).ToList();
    }

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

    private LoanResponseDto MapToDto(Loan l) =>
        new LoanResponseDto(l.Id, l.TotalAmount, l.RemainingAmount, l.InterestRate, l.StartDate, l.PerSecondPayment, l.IsPaid);
}