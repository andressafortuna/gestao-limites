using FraudSys.Application.DTOs;

namespace FraudSys.Application.Interfaces
{
    public interface IPixTransactionService
    {
        Task<PixTransactionResponseDto> ProcessTransactionAsync(ProcessPixTransactionDto dto);
    }
}
