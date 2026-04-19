using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBankApplication.Data;
using WebBankApplication.DTOs;
using WebBankApplication.Models;
using WebBankApplication.Repository;

public class RemittanceRepository : IRemittanceRepository
{
    private readonly AppDbContext _context;

    public RemittanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RemittanceAddAsync(RemittanceAddDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var sender = await _context.Users.FindAsync(dto.SenderId);
            var recipient = await _context.Users.FindAsync(dto.RecipientId);

            if (sender == null || recipient == null || recipient == sender || sender.Balance < dto.Amount)
                return false;

            sender.Balance -= dto.Amount;
            recipient.Balance += dto.Amount;


            var remittance = new Remittance
            {
                SenderId = dto.SenderId,
                RecipientId = dto.RecipientId,
                Amount = dto.Amount
            };

            _context.Remittances.Add(remittance);


            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<List<RemittanceHistoryDto>> GetRemittanceHistoryAsync(Guid userId)
    {
        return await _context.Remittances
            .Where(r => r.SenderId == userId || r.RecipientId == userId)
            .OrderByDescending(r => r.Date)
            .Select(r => new RemittanceHistoryDto(
                Id: r.Id,
                CounterpartyFullName: r.SenderId == userId ? r.Recipient.FullName : r.Sender.FullName,
                Amount: r.Amount,
                Date: r.Date,
                IsIncoming: r.RecipientId == userId
            ))
            .ToListAsync();
    }
}