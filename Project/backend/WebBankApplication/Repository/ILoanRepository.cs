using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBankApplication.DTOs;

namespace WebBankApplication.Repository;

public interface ILoanRepository
{
    Task<ResponseLoanDto> AddLoan(AddLoanDto dto);
    Task<List<ResponseLoanDto>> GetLoans(Guid userId);
    Task ProcessLoanPayments();
}
