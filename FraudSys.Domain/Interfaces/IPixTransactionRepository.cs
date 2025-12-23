using FraudSys.Domain.Entities;

namespace FraudSys.Domain.Interfaces
{
    public interface IPixTransactionRepository
    {
        Task SaveAsync(PixTransaction transaction);
        Task<PixTransaction?> GetByIdAsync(string transactionId);
        Task<IEnumerable<PixTransaction>> GetByDocumentAsync(string document);
    }
}
