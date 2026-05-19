using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Lexicon_Miniproject_SQLAssetTracking
{
    

    internal static class PriceConverter
    {
        struct EuroToCurrencyConversion
        {
            public string CurrencyCode { get; set; } = "";
            public decimal Rate { get; set; } = 0;

            public EuroToCurrencyConversion(string aCurrencyCode, decimal aRate)
            {
                CurrencyCode = aCurrencyCode;
                Rate = aRate;
            }
        }
        static private string XmlUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
        static private List<EuroToCurrencyConversion> LoadedConversions { get; set; } = new List<EuroToCurrencyConversion>();
        static private DateOnly CurrencyDocUpdated { get; set; } = new DateOnly();

        public static decimal ConvertToEuro(Price aPrice) 
        {
            foreach (var conversion in LoadedConversions)
            {
                if (conversion.CurrencyCode == aPrice.CurrencyCode)
                {
                    return aPrice.Value / conversion.Rate;
                }
            }

            return -1;
        }

        public static decimal ConvertFromEuro(decimal aEuroValue, string aTargetCurrencyCode) 
        {
            foreach (var conversion in LoadedConversions)
            {
                if (conversion.CurrencyCode == aTargetCurrencyCode)
                {
                    return aEuroValue * conversion.Rate;
                }
            }

            return -1;
        }

        public static void LoadConversions()
        {
            XmlReader xmlReader = XmlReader.Create(XmlUrl);

            LoadedConversions.Clear();
            while (xmlReader.Read())
            {
                if (!string.IsNullOrEmpty(xmlReader.GetAttribute("time")))
                {
                    string timeAttribute = xmlReader.GetAttribute("time");
                    CurrencyDocUpdated = DateOnly.Parse(timeAttribute);
                }

                if (xmlReader.Name == "Cube" && !string.IsNullOrEmpty(xmlReader.GetAttribute("currency")))
                {
                    string currencyType = xmlReader.GetAttribute("currency");
                    string rateStr = xmlReader.GetAttribute("rate");
                    LoadedConversions.Add(new EuroToCurrencyConversion(currencyType, decimal.Parse(rateStr)));
                }
            }
        }
    }
}
