using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon_Miniproject_SQLAssetTracking
{
    internal abstract class Device
    {
        public DBDevice? DBRef = null;
        public string DeviceBrand { get; set; }
        public string ModelName { get; set; }
        public string OfficeLocation { get; set; }
        public string SerialNumber { get; set; }
        public DateOnly PurchaseDate { get; set; }
        public Price PurchasePrice { get; set; }
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
                return PurchasePrice.GetLocalValue();
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
