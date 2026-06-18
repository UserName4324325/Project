using System;

namespace WebBankApplication.DTOs;

public record AddLoanDto(

    Guid UserId,
    decimal Amount,
    int TermInSeconds,
    float InterestRate

);


public record ResponseLoanDto(

    Guid Id,
    int TermInSeconds,
    decimal TotalAmount,
    decimal RemainingAmount,
    float InterestRate,
    DateTime StartDate,
    decimal PerSecondPayment,
    bool IsPaid

);