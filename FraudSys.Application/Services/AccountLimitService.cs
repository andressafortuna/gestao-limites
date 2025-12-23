using AutoMapper;
using FraudSys.Application.DTOs;
using FraudSys.Application.Interfaces;
using FraudSys.Domain.Entities;
using FraudSys.Domain.Exceptions;
using FraudSys.Domain.Interfaces;

namespace FraudSys.Application.Services
{
    public class AccountLimitService : IAccountLimitService
    {
        private readonly IAccountLimitRepository _repository;
        private readonly IMapper _mapper;

        public AccountLimitService(IAccountLimitRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AccountLimitResponseDto> CreateAsync(CreateAccountLimitDto dto)
        {
            var exists = await _repository.ExistsAsync(dto.Document);
            if (exists)
            {
                throw new DomainException($"Já existe um limite cadastrado para o CPF {dto.Document}");
            }

            var accountLimit = new AccountLimit(
                dto.Document,
                dto.AgencyNumber,
                dto.AccountNumber,
                dto.PixLimit
            );

            await _repository.CreateAsync(accountLimit);

            return _mapper.Map<AccountLimitResponseDto>(accountLimit);
        }

        public async Task<AccountLimitResponseDto> GetByDocumentAsync(string document)
        {
            var accountLimit = await _repository.GetByDocumentAsync(document);

            if (accountLimit == null)
            {
                throw new AccountLimitNotFoundException(document);
            }

            return _mapper.Map<AccountLimitResponseDto>(accountLimit);
        }

        public async Task<AccountLimitResponseDto> UpdateAsync(string document, UpdateAccountLimitDto dto)
        {
            var accountLimit = await _repository.GetByDocumentAsync(document);

            if (accountLimit == null)
            {
                throw new AccountLimitNotFoundException(document);
            }

            accountLimit.UpdateLimit(dto.NewPixLimit);

            await _repository.UpdateAsync(accountLimit);

            return _mapper.Map<AccountLimitResponseDto>(accountLimit);
        }

        public async Task DeleteAsync(string document)
        {
            var accountLimit = await _repository.GetByDocumentAsync(document);

            if (accountLimit == null)
            {
                throw new AccountLimitNotFoundException(document);
            }

            await _repository.DeleteAsync(document);
        }
    }
}
