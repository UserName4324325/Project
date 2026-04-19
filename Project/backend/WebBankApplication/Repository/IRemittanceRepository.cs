using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBankApplication.DTOs;

namespace WebBankApplication.Repository;

public interface IRemittanceRepository
{
    Task<bool> RemittanceAddAsync(RemittanceAddDto dto); 
    Task<List<RemittanceHistoryDto>> GetRemittanceHistoryAsync(Guid userId);
}
