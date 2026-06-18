using System;

namespace WebBankApplication.DTOs;

public record AddRemittanceDto  
(
    Guid SenderId,
    Guid RecipientId,
    decimal Amount
);

public record ResponseRemittanceDto
(
    Guid Id,
    string CounterpartyFullName,
    decimal Amount,
    DateTime Date,
    bool IsIncoming // true — зачисление, false — перевод
);
