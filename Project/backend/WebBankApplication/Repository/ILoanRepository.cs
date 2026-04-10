using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBankApplication.DTOs;

namespace WebBankApplication.Repository;

public interface ILoanRepository
{
    Task<LoanResponseDto> TakeLoan(TakeLoanDto dto);
    Task<List<LoanResponseDto>> GetUserLoans(Guid userId);
    Task ProcessLoanPayments();
}
