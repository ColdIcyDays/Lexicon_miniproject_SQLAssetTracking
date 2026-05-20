using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon_Miniproject_SQLAssetTracking
{
    internal abstract class Device
    {
        public string DeviceBrand { get; private set; }
        public string ModelName { get; private set; }
        public string OfficeLocation { get; private set; }
        public string SerialNumber { get; private set; }
        public DateOnly PurchaseDate { get; private set; }
        public Price PurchasePrice { get; private set; }
        public Device(Price aPurchasePrice, DateOnly aPurchaseDate, string aDeviceBrand, string aModelName, string aOfficeLocation, string aSerialNumber) 
        {
            PurchasePrice = aPurchasePrice;
            PurchaseDate = aPurchaseDate;
            DeviceBrand = aDeviceBrand;
            ModelName = aModelName;
            OfficeLocation = aOfficeLocation.ToUpper();
            SerialNumber = aSerialNumber;
        }

        public decimal GetPriceInCurrency(string aCurrencyCode)
        {
            if (PurchasePrice.CurrencyCode == aCurrencyCode)
            {
                return PurchasePrice.Value;
            }

            if (aCurrencyCode == "EURO")
            {
                return PriceConverter.ConvertToEuro(PurchasePrice);
            }

            return PriceConverter.ConvertFromEuro(PriceConverter.ConvertToEuro(PurchasePrice), aCurrencyCode);
        }
        public abstract string GetDeviceType();
    }
}
