using Lexicon_StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Lexicon_ConsoleWriter;

namespace Lexicon_Miniproject_SQLAssetTracking
{
    internal class AssetTrackingState : BaseState
    {
        private List<Device> DevicesToTrack { get; set; } = new List<Device>();
        public AssetTrackingState(LexiStateMachine aOwningStateMachine) : base(aOwningStateMachine) {}

        public override void CleanupState() {}

        public override void RunState()
        {
            const int padding = 15;
            LexiConsoleWriter.LexiWriteLine("AssetType".PadRight(padding) + "Office".PadRight(padding) + "Brand".PadRight(padding) + "Model".PadRight(padding) + "Purchase Date".PadRight(padding) + "Price in USD".PadRight(padding) + "Currency".PadRight(padding) + "Local price today".PadRight(padding));
            LexiConsoleWriter.LexiWriteLine("---------".PadRight(padding) + "------".PadRight(padding) + "-----".PadRight(padding) + "-----".PadRight(padding) + "-------------".PadRight(padding) + "------------".PadRight(padding) + "--------".PadRight(padding) + "-----------------".PadRight(padding));
            
            foreach (Device device in DevicesToTrack)
            {
                DateTime date = device.PurchaseDate.ToDateTime(new TimeOnly());
                date = date.AddYears(3);
                double monthsTilExpiration = date.Subtract(DateTime.Now).TotalDays / 30;

                //Console.BackgroundColor = ConsoleColor.Gray;
                if (monthsTilExpiration < 3)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (monthsTilExpiration < 6)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write(device.GetDeviceType().PadRight(padding));
                Console.Write(device.OfficeLocation.PadRight(padding));
                Console.Write(device.DeviceBrand.PadRight(padding));
                Console.Write(device.ModelName.PadRight(padding));
                Console.Write(device.PurchaseDate.ToString().PadRight(padding));
                Console.Write(device.GetPriceInCurrency("USD").ToString(".##").PadRight(padding));
                Console.Write(device.PurchasePrice.CurrencyCode.PadRight(padding));
                Console.Write(device.PurchasePrice.Value.ToString(".##").PadRight(padding));
                Console.Write("\n");
                Console.ResetColor();
            }

            LexiConsoleWriter.LexiWriteLine("Type q to quit.");

            string lowercaseInput = "";
            do
            {
                ReadUserInput(out lowercaseInput);
            } while (lowercaseInput != "q");

            StateMachine.PopState();
        }

        public override void SetupState()
        {
            DevicesToTrack.Add(new SmartphoneDevice(new Price(400, "USD"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 5)), "Motorola", "X3", "USA"));
            DevicesToTrack.Add(new SmartphoneDevice(new Price(500, "USD"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 4)), "Motorola", "X6", "USA"));
            DevicesToTrack.Add(new SmartphoneDevice(new Price(3200, "SEK"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 2)), "Apple", "Pro 5", "SWEDEN"));
            DevicesToTrack.Add(new SmartphoneDevice(new Price(10000, "JPY"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 8)), "Samsung", "Galaxy 40", "JAPAN"));
            DevicesToTrack.Add(new SmartphoneDevice(new Price(1100, "USD"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 5)), "Motorola", "X7000", "USA"));

            DevicesToTrack.Add(new ComputerDevice(new Price(150, "SEK"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 13)), "ASUS", "Zenbook S", "SWEDEN"));
            DevicesToTrack.Add(new ComputerDevice(new Price(300, "USD"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 2)), "ASUS", "Zenbook Z", "USA"));
            DevicesToTrack.Add(new ComputerDevice(new Price(4400, "SEK"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 1)), "HP", "OmniBook 70", "SWEDEN"));
            DevicesToTrack.Add(new ComputerDevice(new Price(40000, "JPY"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 30)), "HP", "OmniBook 5", "JAPAN"));
            DevicesToTrack.Add(new ComputerDevice(new Price(1500, "USD"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 34)), "Dell", "X7000", "USA"));

            DevicesToTrack.Sort(CompareByOfficeAndPurchaseDate);
        }

        static private int CompareByOfficeAndPurchaseDate(Device aDevice1, Device aDevice2)
        {
            if (aDevice1.OfficeLocation != aDevice2.OfficeLocation)
            {
                return aDevice1.OfficeLocation[0] > aDevice2.OfficeLocation[0] ? 1 : -1;
            }

            if (aDevice1.PurchaseDate != aDevice2.PurchaseDate)
            {
                return aDevice1.PurchaseDate > aDevice2.PurchaseDate ? 1 : -1;
            }
            
            return 0;
        }
    }
}
