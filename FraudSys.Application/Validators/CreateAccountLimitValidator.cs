using FluentValidation;
using FraudSys.Application.DTOs;

namespace FraudSys.Application.Validators
{
    public class CreateAccountLimitValidator : AbstractValidator<CreateAccountLimitDto>
    {
        public CreateAccountLimitValidator()
        {
            RuleFor(x => x.Document)
                .NotEmpty().WithMessage("CPF é obrigatório")
                .Must(BeValidCpf).WithMessage("CPF inválido");

            RuleFor(x => x.AgencyNumber)
                .NotEmpty().WithMessage("Número da agência é obrigatório");

            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("Número da conta é obrigatório");

            RuleFor(x => x.PixLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Limite PIX deve ser maior ou igual a zero");
        }

        private bool BeValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return false;

            if (cpf.Distinct().Count() == 1)
                return false;

            var sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(cpf[i].ToString()) * (10 - i);

            var remainder = sum % 11;
            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cpf[9].ToString()) != digit1)
                return false;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(cpf[i].ToString()) * (11 - i);

            remainder = sum % 11;
            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            return int.Parse(cpf[10].ToString()) == digit2;
        }
    }
}
