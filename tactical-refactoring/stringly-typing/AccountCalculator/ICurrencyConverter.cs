using AccountCalculator.Domain;

namespace AccountCalculator
{

    public interface ICurrencyConverter
    {
        decimal ConvertCurrency(
            decimal originalValue,
            string originalCurrency,
            string targetCurrency,
            UtcDateTime timeOfConversion);
    }
}
