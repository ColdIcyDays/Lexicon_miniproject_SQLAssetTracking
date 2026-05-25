using System;
using System.Collections.Generic;
using System.Globalization;
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
            return ConvertToEuro(aPrice.GetLocalValue(), aPrice.CurrencyCode); 
            /*foreach (var conversion in LoadedConversions)
            {
                if (conversion.CurrencyCode == aPrice.CurrencyCode)
                {
                    return aPrice.Value / conversion.Rate;
                }
            }

            return -1;*/
        }
        
        public static decimal ConvertToEuro(decimal aValue, string aValueCurrencyCode) 
        {
            foreach (var conversion in LoadedConversions)
            {
                if (conversion.CurrencyCode == aValueCurrencyCode)
                {
                    return aValue / conversion.Rate;
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

        public static bool IsValidCurrencyCode(string aCurrencyCode)
        {
            foreach (var conversion in LoadedConversions)
            {
                if (conversion.CurrencyCode == aCurrencyCode)
                {
                    return true;
                }
            }

            return false;
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

        
        static private Dictionary<string, string> CodeToSymbols = new Dictionary<string, string>();
        
        static private Dictionary<string, NumberFormatInfo> CodeToCurrencyPos = new Dictionary<string, NumberFormatInfo>();
        
        public static void LoadCodeToSymbols()
        {
            CodeToSymbols = new Dictionary<string, string>();

            /*var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(x => new RegionInfo(x.LCID));*/

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            
            /*foreach (var region in regions)
                if (!CodeToSymbols.ContainsKey(region.ISOCurrencySymbol))
                    CodeToSymbols.Add(region.ISOCurrencySymbol, region.CurrencySymbol);*/

            foreach (var culture in cultures )
            {
                try
                {
                    var region = new RegionInfo(culture.LCID);
                    if (!CodeToCurrencyPos.ContainsKey(region.ISOCurrencySymbol))
                    {
                        CodeToCurrencyPos.Add(region.ISOCurrencySymbol, culture.NumberFormat);
                    }
                }
                catch
                {
                    
                }
            }
        }

        /*public static string GetSymbol(string aCurrencyCode)
        {
            return CodeToSymbols[aCurrencyCode];
        }*/

        public static string FormatCurrency(decimal aNumber, string aCurrencyCode, string aDecimalFormat = ".##")
        {
            string decimaledNumber = aNumber.ToString(aDecimalFormat);

            if (CodeToCurrencyPos.ContainsKey(aCurrencyCode))
            {
                return decimal.Parse(decimaledNumber).ToString("C", CodeToCurrencyPos[aCurrencyCode]);
            }

            return decimaledNumber + "???";
        }
    }
}
