using System;

namespace WebBankApplication.DTOs;

public record TakeLoanDto(

    Guid UserId,
    decimal Amount,
    int TermInSeconds,
    float InterestRate

);


public record LoanResponseDto(

    Guid Id,
    int TermInSeconds,
    decimal TotalAmount,
    decimal RemainingAmount,
    float InterestRate,
    DateTime StartDate,
    decimal PerSecondPayment,
    bool IsPaid

);