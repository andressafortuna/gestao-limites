using AutoMapper;
using FraudSys.Application.DTOs;
using FraudSys.Application.Interfaces;
using FraudSys.Domain.Entities;
using FraudSys.Domain.Exceptions;
using FraudSys.Domain.Interfaces;

namespace FraudSys.Application.Services
{
    public class PixTransactionService : IPixTransactionService
    {
        private readonly IAccountLimitRepository _accountLimitRepository;
        private readonly IPixTransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public PixTransactionService(
            IAccountLimitRepository accountLimitRepository,
            IPixTransactionRepository transactionRepository,
            IMapper mapper)
        {
            _accountLimitRepository = accountLimitRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<PixTransactionResponseDto> ProcessTransactionAsync(ProcessPixTransactionDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.TransactionId))
            {
                var existingTransaction = await _transactionRepository.GetByIdAsync(dto.TransactionId);
                if (existingTransaction != null)
                {
                    var response = _mapper.Map<PixTransactionResponseDto>(existingTransaction);
                    response.RemainingLimit = 0; 
                    return response;
                }
            }

            var accountLimit = await _accountLimitRepository.GetByDocumentAsync(dto.Document);

            if (accountLimit == null)
            {
                throw new AccountLimitNotFoundException(dto.Document);
            }

            if (accountLimit.AgencyNumber != dto.AgencyNumber || accountLimit.AccountNumber != dto.AccountNumber)
            {
                throw new DomainException("Agência ou conta não correspondem ao CPF informado");
            }

            var transaction = new PixTransaction(
                dto.TransactionId ?? Guid.NewGuid().ToString(),
                dto.Document,
                dto.AgencyNumber,
                dto.AccountNumber,
                dto.Amount
            );

            try
            {
                accountLimit.ConsumeLimit(dto.Amount);

                transaction.Approve();

                await _accountLimitRepository.UpdateAsync(accountLimit);

                await _transactionRepository.SaveAsync(transaction);

                var responseDto = _mapper.Map<PixTransactionResponseDto>(transaction);
                responseDto.RemainingLimit = accountLimit.PixLimit.Value;

                return responseDto;
            }
            catch (InsufficientLimitException ex)
            {
                transaction.Deny(ex.Message);

                await _transactionRepository.SaveAsync(transaction);

                var responseDto = _mapper.Map<PixTransactionResponseDto>(transaction);
                responseDto.RemainingLimit = accountLimit.PixLimit.Value;

                return responseDto;
            }
        }
    }
}
