using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountCalculator.Domain;

namespace AccountCalculator
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Usage: AccountCalculator.exe <path-to-purchases-file> <currency-code>");
                Environment.Exit(1);
            }

            try
            {
                var purchasesFile = new FileInfo(args[0]);
                var commonCurrency = new Currency(args[1]);

                CalculatePurchases(purchasesFile, commonCurrency, Console.WriteLine, Console.Error.WriteLine).Wait();
            }
            catch (Exception exception)
            {
                Console.Error.Write(exception);
                Environment.Exit(1);
            }
        }

        private static async Task CalculatePurchases(
            FileInfo purchasesFile,
            Currency commonCurrency,
            Action<string> writeOutput,
            Action<string> writeInfo)
        {
            // Initialise the currency converter.
            var exchangeRatesProvider = new ExchangeRatesProvider(DownloadRawExchangeRateData);
            var converter = new CurrencyConverter(exchangeRatesProvider);

            // Load the purchases from the purchase file.
            var purchasesReader = new FilePurchasesReader(purchasesFile);
            var purchases = await purchasesReader.ReadPurchases();

            // Convert all the purchases to the same common currency.
            var purchasesInGbp = purchases
                .Select(x => new Purchase(
                    x.Timestamp,
                    x.Description,
                    converter.ConvertCurrency(x.Cost, commonCurrency, x.Timestamp)))
                .ToList();

            foreach (var purchase in purchasesInGbp)
            {
                writeOutput($"{purchase.Timestamp:yyyy-MM-ddThh:mm:sszzz},{purchase.Description},{purchase.Cost}");
            }

            var totalCost = purchasesInGbp.Sum(purchase => purchase.Cost.Amount);
            writeInfo($"Total cost is {new Money(totalCost, commonCurrency)}");
        }

        private static Task<string> DownloadRawExchangeRateData()
        {
            var currencyConversionsUri =
                "https://gist.githubusercontent.com/ChrisLambrou/495497ab11487c9421a5eed40026a30f/raw/f06136e951491f6918913f73974c074ae21db5a2/CurrencyConversions2021.csv";
            var downloader = new JsonDownloader();
            return downloader.Download(new Uri(currencyConversionsUri), CancellationToken.None);
        }
    }
}
