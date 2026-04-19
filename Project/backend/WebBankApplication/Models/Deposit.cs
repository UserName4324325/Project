using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBankApplication.Models;

public record Deposit
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public float InterestRate { get; set; }
    public int TermInSeconds { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public bool IsClosed { get; set; } = false;
    public decimal Profit { get; set; }



    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }
}
