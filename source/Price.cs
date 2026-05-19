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
            Value = 0;
            CurrencyCode = "USD";
        }

        public Price(decimal aValue)
        {
            Value = aValue;
            CurrencyCode = "USD";
        }

        public Price(decimal aValue, string aCurrencyCode) : this(aValue)
        {
            CurrencyCode = aCurrencyCode;
        }

        public decimal Value { get; set; }
        public string CurrencyCode { get; set; }
        
        
    }
}
