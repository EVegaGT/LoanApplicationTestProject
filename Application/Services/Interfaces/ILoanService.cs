using Application.DTOS;
using Domain.Models;

namespace Application.Services.Interfaces
{
    public interface ILoanService
    {
        Task<RequestLoanApplication?> GetCustomerBySSNAsync(string ssn);
        Task<ResponseResult> ProcessApplicationAsync(RequestLoanApplication request);
    }
}
