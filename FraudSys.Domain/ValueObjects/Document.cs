namespace FraudSys.Domain.ValueObjects
{
    public class Document
    {
        public string Value { get; private set; }

        public Document(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exceptions.InvalidDocumentException("CPF não pode ser vazio");

            var cleanedValue = CleanDocument(value);

            if (!IsValid(cleanedValue))
                throw new Exceptions.InvalidDocumentException(value);

            Value = cleanedValue;
        }

        private string CleanDocument(string document)
        {
            return new string(document.Where(char.IsDigit).ToArray());
        }

        private bool IsValid(string cpf)
        {
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

        public override string ToString() => Value;

        public static implicit operator string(Document document) => document.Value;
    }
}
