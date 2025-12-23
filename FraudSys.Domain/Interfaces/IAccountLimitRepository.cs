using FraudSys.Domain.Entities;

namespace FraudSys.Domain.Interfaces
{
    public interface IAccountLimitRepository
    {
        Task<AccountLimit?> GetByDocumentAsync(string document);
        Task CreateAsync(AccountLimit accountLimit);
        Task UpdateAsync(AccountLimit accountLimit);
        Task DeleteAsync(string document);
        Task<bool> ExistsAsync(string document);
    }
}
