using AccountCalculator.Domain;

namespace AccountCalculator
{

    public interface ICurrencyConverter
    {
        Money ConvertCurrency(Money originalMoney, string targetCurrency, UtcDateTime timeOfConversion);
    }
}
