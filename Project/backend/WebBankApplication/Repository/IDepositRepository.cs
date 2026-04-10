using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBankApplication.DTOs;

namespace WebBankApplication.Repository;

public interface IDepositRepository
{
    Task<DepositResponseDto> OpenDeposit(OpenDepositDto dto);
    Task<List<DepositResponseDto>> GetUserDeposits(Guid userId);
    Task ProcessExpiredDeposits();
}
