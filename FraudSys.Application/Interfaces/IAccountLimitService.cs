using FraudSys.Application.DTOs;

namespace FraudSys.Application.Interfaces
{
    public interface IAccountLimitService
    {
        Task<AccountLimitResponseDto> CreateAsync(CreateAccountLimitDto dto);
        Task<AccountLimitResponseDto> GetByDocumentAsync(string document);
        Task<AccountLimitResponseDto> UpdateAsync(string document, UpdateAccountLimitDto dto);
        Task DeleteAsync(string document);
    }
}
