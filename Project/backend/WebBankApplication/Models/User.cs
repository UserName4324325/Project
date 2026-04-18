using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBankApplication.Models;

public record User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(40)]
    public string FullName { get; set; } = string.Empty;
    [MaxLength(40)]
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public decimal Balance { get; set; }


    public List<Deposit>? Deposits { get; set; } = [];
    public List<Loan>? Loans { get; set; } = [];
}
