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

public class LoanRepository : ILoanRepository
{
    private readonly AppDbContext _context;

    public LoanRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseLoanDto> AddLoan(AddLoanDto dto) 
    {
        var user = await _context.Users.FindAsync(dto.UserId);

        if (user == null) throw new Exception("Пользователь не найден");

        // АННУИТЕТНАЯ МАТЕМАТИКА
        decimal monthlyRate = (decimal)dto.InterestRate / 12m / 100m;
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

        return MapToResponseLoanDto(loan);
    }
    private ResponseLoanDto MapToResponseLoanDto(Loan l) =>
        new ResponseLoanDto(l.Id, l.TermInSeconds, l.TotalAmount, l.RemainingAmount, l.InterestRate, l.StartDate, l.PerSecondPayment, l.IsPaid);

    public async Task<List<ResponseLoanDto>> GetLoans(Guid userId)
    {
        return await _context.Loans
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.StartDate)
            .Select(AsResponseLoanDto)
            .ToListAsync();
    }

    private static readonly Expression<Func<Loan, ResponseLoanDto>> AsResponseLoanDto = l =>
    new ResponseLoanDto
    (
        l.Id,
        l.TermInSeconds,
        l.TotalAmount,
        l.RemainingAmount,
        l.InterestRate,
        l.StartDate,
        l.PerSecondPayment,
        l.IsPaid
    );

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

            if (loan.RemainingAmount <= 0.05m)
            {
                loan.RemainingAmount = 0;
                loan.IsPaid = true;
            }
        }

        await _context.SaveChangesAsync();
    }

}