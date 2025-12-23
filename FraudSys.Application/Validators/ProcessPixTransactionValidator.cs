using FluentValidation;
using FraudSys.Application.DTOs;

namespace FraudSys.Application.Validators
{
    public class ProcessPixTransactionValidator : AbstractValidator<ProcessPixTransactionDto>
    {
        public ProcessPixTransactionValidator()
        {
            RuleFor(x => x.Document)
                .NotEmpty().WithMessage("CPF é obrigatório");

            RuleFor(x => x.AgencyNumber)
                .NotEmpty().WithMessage("Número da agência é obrigatório");

            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("Número da conta é obrigatório");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Valor da transação deve ser maior que zero");
        }
    }
}
