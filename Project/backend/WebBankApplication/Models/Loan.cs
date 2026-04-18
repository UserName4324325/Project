using System;

namespace WebBankApplication.Models;

public record Loan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal TotalAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public float InterestRate { get; set; }
    public int TermInSeconds { get; set; }
    public decimal PerSecondPayment { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public bool IsPaid { get; set; } = false;



    public Guid UserId { get; set; }
    public User? User { get; set; }
}
