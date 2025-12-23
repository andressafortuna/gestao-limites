using FluentValidation;
using FraudSys.Application.DTOs;

namespace FraudSys.Application.Validators
{
    public class UpdateAccountLimitValidator : AbstractValidator<UpdateAccountLimitDto>
    {
        public UpdateAccountLimitValidator()
        {
            RuleFor(x => x.NewPixLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Novo limite PIX deve ser maior ou igual a zero");
        }
    }
}
