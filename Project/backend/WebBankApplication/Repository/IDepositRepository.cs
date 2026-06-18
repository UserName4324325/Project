using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBankApplication.DTOs;

namespace WebBankApplication.Repository;

public interface IDepositRepository
{
    Task<ResponseDepositDto> AddDeposit(AddDepositDto dto);
    Task<List<ResponseDepositDto>> GetDeposits(Guid userId);
    Task ProcessExpiredDeposits();
}
