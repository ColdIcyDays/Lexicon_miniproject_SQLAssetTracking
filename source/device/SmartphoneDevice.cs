using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon_Miniproject_SQLAssetTracking
{
    internal class SmartphoneDevice : Device
    {
        public SmartphoneDevice(Price aPurchasePrice, DateOnly aPurchaseDate, string aDeviceBrand, string aModelName, string aOfficeLocation, string aSerialNumber) 
            : base(aPurchasePrice, aPurchaseDate, aDeviceBrand, aModelName, aOfficeLocation, aSerialNumber) {}

        public override string GetDeviceType()
        {
            return "Smartphone";
        }
    }
}
