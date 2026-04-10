using System;

namespace WebBankApplication.DTOs;

public record OpenDepositDto(

    Guid UserId,
    decimal Amount,
    int TermInSeconds,
    float InterestRate

);
public record DepositResponseDto(

    Guid Id,
    decimal Amount,
    float InterestRate,
    int TermInSeconds,
    DateTime StartDate,
    decimal Profit,
    bool IsClosed

);