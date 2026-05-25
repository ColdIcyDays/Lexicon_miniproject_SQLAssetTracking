using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon_Miniproject_SQLAssetTracking
{
    internal struct Price
    {
        public Price()
        {
            ValueInUSD = 0;
            CurrencyCode = "USD";
        }

        public Price(decimal aValue)
        {
            ValueInUSD = aValue;
            CurrencyCode = "USD";
        }

        public Price(decimal aValueInUSD, string aCurrencyCode) : this(aValueInUSD)
        {
            CurrencyCode = aCurrencyCode;

            /*if (CurrencyCode != "USD")
            {
                Value = PriceConverter.ConvertFromEuro(PriceConverter.ConvertToEuro(aValueInUSD, "USD"), aCurrencyCode);
            }*/
        }

        public decimal GetLocalValue()
        {
            return PriceConverter.ConvertFromEuro(PriceConverter.ConvertToEuro(ValueInUSD, "USD"), CurrencyCode);
        }

        public decimal GetValueAsUSD()
        {
            return ValueInUSD;
        }

        private decimal ValueInUSD { get; set; }
        public string CurrencyCode { get; set; }
        
        
    }
}
