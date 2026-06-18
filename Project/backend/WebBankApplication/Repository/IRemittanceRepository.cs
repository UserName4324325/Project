using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBankApplication.DTOs;

namespace WebBankApplication.Repository;

public interface IRemittanceRepository
{
    Task<bool> AddRemittanceAsync(AddRemittanceDto dto); 
    Task<List<ResponseRemittanceDto>> GetRemittanceHistoryAsync(Guid userId);
}
