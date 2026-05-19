using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon_Miniproject_SQLAssetTracking
{
    internal class ComputerDevice : Device
    {
        public ComputerDevice(Price aPurchasePrice, DateOnly aPurchaseDate, string aDeviceBrand, string aModelName, string aOfficeLocation) 
            : base(aPurchasePrice, aPurchaseDate, aDeviceBrand, aModelName, aOfficeLocation) {}

        public override string GetDeviceType()
        {
            return "Computer";
        }
    }
}
