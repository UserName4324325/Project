using System;

namespace WebBankApplication.DTOs;

public record RemittanceAddDto  
(
    Guid SenderId,
    Guid RecipientId,
    decimal Amount
);

public record RemittanceHistoryDto
(
    Guid Id,
    string CounterpartyFullName,
    decimal Amount,
    DateTime Date,
    bool IsIncoming // true — зачисление, false — перевод
);
