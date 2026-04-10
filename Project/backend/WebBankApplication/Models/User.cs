using System;
using System.Collections.Generic;

namespace WebBankApplication.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public decimal Balance { get; set; }


    public List<Deposit>? Deposits { get; set; } = [];
    public List<Loan>? Loans { get; set; } = [];
}
