using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBankApplication.Models;

public record User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(40)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(40)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public decimal Balance { get; set; }



    public virtual List<Deposit> Deposits { get; set; } = [];
    public virtual List<Loan> Loans { get; set; } = [];


    public virtual List<Remittance> SentRemittances { get; set; } = [];
    public virtual List<Remittance> ReceivedRemittances { get; set; } = [];
}
