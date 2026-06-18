using System;

namespace WebBankApplication.DTOs;

public record AddDepositDto(

    Guid UserId,
    decimal Amount,
    int TermInSeconds,
    float InterestRate

);
public record ResponseDepositDto(

    Guid Id,
    decimal Amount,
    float InterestRate,
    int TermInSeconds,
    DateTime StartDate,
    decimal Profit,
    bool IsClosed

);