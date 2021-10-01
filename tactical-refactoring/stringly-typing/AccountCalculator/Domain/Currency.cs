using System;

namespace AccountCalculator.Domain
{
    /// <summary>
    /// Represents a currency, characterised by a three letter <see cref="CurrencyCode"/>, such as GBP or USD.
    /// </summary>
    /// <param name="CurrencyCode">The three letter currency code of this currency, such as GBP or USD.</param>
    public record Currency(string CurrencyCode) : IComparable<Currency>
    {
        public override string ToString() => CurrencyCode;

        public int CompareTo(Currency? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(CurrencyCode, other.CurrencyCode, StringComparison.Ordinal);
        }
    }
}
