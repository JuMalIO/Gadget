using System.Collections.Generic;

namespace Gadget.Widgets.Currency
{
    public sealed class CurrencyData
    {
        public bool Visible { get; set; }
        public string Currency { get; set; }
        public string CurrencyShort { get; set; }
        public Dictionary<CurrencyType, string> Money { get; set; } = new Dictionary<CurrencyType, string>();

        public CurrencyData()
        {
        }

        public CurrencyData(bool visible, string currency, string currencyShort)
        {
            Visible = visible;
            Currency = currency;
            CurrencyShort = currencyShort;
        }
    }
}
