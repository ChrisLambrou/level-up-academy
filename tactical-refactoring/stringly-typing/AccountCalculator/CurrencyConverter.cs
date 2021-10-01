using System;
using System.Linq;
using System.Threading.Tasks;
using AccountCalculator.Domain;

namespace AccountCalculator
{
    public sealed class CurrencyConverter : ICurrencyConverter
    {
        private record ConversionRate(UtcDateTime Start, UtcDateTime End, decimal Rate);

        private readonly Task<ILookup<string, ConversionRate>> _conversionRates;

        public CurrencyConverter(IExchangeRatesProvider exchangeRatesProvider) =>
            _conversionRates = InitializeConversionRates(exchangeRatesProvider);

        private static async Task<ILookup<string, ConversionRate>> InitializeConversionRates(
            IExchangeRatesProvider exchangeRatesProvider) =>
            (await exchangeRatesProvider.GetExchangeRates())
            .OrderBy(x => x.Start)
            .ThenBy(x => x.Currency)
            .ToLookup(x => x.Currency, x => new ConversionRate(x.Start, x.End, x.ConversionRate));

        public Money ConvertCurrency(Money originalMoney, string targetCurrency, UtcDateTime timeOfConversion)
        {
            if (originalMoney.Currency == targetCurrency)
            {
                return originalMoney; // No conversion required.
            }

            var rateFromOriginalToGbp = GetConversionRate(originalMoney.Currency, timeOfConversion);
            var rateFromTargetToGbp = GetConversionRate(targetCurrency, timeOfConversion);

            var newAmount = originalMoney.Amount * (rateFromTargetToGbp / rateFromOriginalToGbp);
            return new Money(newAmount, targetCurrency);
        }

        private decimal GetConversionRate(string currency, UtcDateTime timeOfConversion)
        {
            if (currency == "GBP")
            {
                return 1;
            }

            var conversionRates = _conversionRates.Result;
            var conversionRate = conversionRates[currency]
                .FirstOrDefault(x => x.Start <= timeOfConversion && timeOfConversion < x.End);
            if (conversionRate == null)
            {
                throw new ArgumentException(
                    $"No conversion available for currency {currency} at {timeOfConversion}");
            }

            return conversionRate.Rate;
        }
    }
}
