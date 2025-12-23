namespace FraudSys.Domain.ValueObjects
{
    public class Money
    {
        public decimal Value { get; private set; }

        public Money(decimal value)
        {
            if (value < 0)
                throw new Exceptions.DomainException("Valor não pode ser negativo");

            Value = value;
        }

        public bool IsGreaterThanOrEqual(Money other)
        {
            return Value >= other.Value;
        }

        public Money Subtract(Money other)
        {
            if (Value < other.Value)
                throw new Exceptions.DomainException("Não é possível subtrair um valor maior");

            return new Money(Value - other.Value);
        }

        public static implicit operator decimal(Money money) => money.Value;

        public override string ToString() => Value.ToString("C");
    }
}
