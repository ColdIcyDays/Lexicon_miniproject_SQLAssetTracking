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
        private AssetTrackingDBContext dbContext = new AssetTrackingDBContext();
        private List<Device> BaseDevices { get; set; } = new List<Device>();
        public AssetTrackingState(LexiStateMachine aOwningStateMachine) : base(aOwningStateMachine) {}

        enum ATInterfaceState
        {
            ATI_Mainmenu,
            ATI_AddAsset,
            ATI_DeleteAsset,
            ATI_UpdateAsset,
            ATI_ShowAssetList,
            ATI_ShowReport,
            ATI_ExitState
        }
        
        public override void SetupState()
        {
            BaseDevices.Add(new SmartphoneDevice(new Price(400, "USD"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 5)), "Motorola", "X3", "USA", "SN-01-0000001"));
            BaseDevices.Add(new SmartphoneDevice(new Price(500, "USD"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 4)), "Motorola", "X6", "USA", "SN-01-0000002"));
            BaseDevices.Add(new SmartphoneDevice(new Price(3200, "SEK"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 2)), "Apple", "Pro 5", "SWEDEN", "SN-01-0000003"));
            BaseDevices.Add(new SmartphoneDevice(new Price(10000, "JPY"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 8)), "Samsung", "Galaxy 40", "JAPAN", "SN-01-0000004"));
            BaseDevices.Add(new SmartphoneDevice(new Price(1100, "USD"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 5)), "Motorola", "X7000", "USA", "SN-01-0000005"));

            BaseDevices.Add(new ComputerDevice(new Price(150, "SEK"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 13)), "ASUS", "Zenbook S", "SWEDEN", "SN-01-0000006"));
            BaseDevices.Add(new ComputerDevice(new Price(300, "USD"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 2)), "ASUS", "Zenbook Z", "USA", "SN-01-0000007"));
            BaseDevices.Add(new ComputerDevice(new Price(4400, "SEK"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 1)), "HP", "OmniBook 70", "SWEDEN", "SN-01-0000008"));
            BaseDevices.Add(new ComputerDevice(new Price(40000, "JPY"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 30)), "HP", "OmniBook 5", "JAPAN", "SN-01-0000009"));
            BaseDevices.Add(new ComputerDevice(new Price(1500, "USD"), DateOnly.FromDateTime(DateTime.Now.Date.AddMonths(-36 + 34)), "Dell", "X7000", "USA", "SN-01-0000010"));
        }

        public override void CleanupState() {}

        protected override List<string> GetLowercaseBackKeywords()
        {
            if (CurrentState == ATInterfaceState.ATI_Mainmenu)
            {
                return new List<string> { "q" };
            }

            return new List<string> { "back" };
        }

        private ATInterfaceState CurrentState = ATInterfaceState.ATI_Mainmenu;
        public override void RunState()
        {

            Console.Clear();
            switch (CurrentState)
            {
                case ATInterfaceState.ATI_Mainmenu:
                    ShowMainMenu();
                    break;
                case ATInterfaceState.ATI_AddAsset:
                    ShowAddAsset();
                    break;
                case ATInterfaceState.ATI_DeleteAsset:
                    ShowDeleteAsset();
                    break;
                case ATInterfaceState.ATI_UpdateAsset:
                    ShowUpdateAsset();
                    break;
                case ATInterfaceState.ATI_ShowAssetList:
                    ShowAssetList();
                    break;
                case ATInterfaceState.ATI_ShowReport:
                    break;
                case ATInterfaceState.ATI_ExitState:
                    StateMachine.PopState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowMainMenu()
        { 
            LexiConsoleWriter.LexiWriteLine("Welcome to the Asset Tracking Interface! (ATI)");
            LexiConsoleWriter.LexiWriteLine("=================================================");
            LexiConsoleWriter.LexiWriteLine("1. Add Asset");
            LexiConsoleWriter.LexiWriteLine("2. Delete Asset");
            LexiConsoleWriter.LexiWriteLine("3. Update Asset");
            LexiConsoleWriter.LexiWriteLine("4. Show Asset List");
            LexiConsoleWriter.LexiWriteLine("5. Show Asset Report");
            LexiConsoleWriter.LexiWriteLine("=================================================");
            LexiConsoleWriter.LexiWriteLine("Type q to quit.");
            
            string lowercaseInput = "";
            do
            {
                if (ReadUserInput(out lowercaseInput))
                {
                    CurrentState = ATInterfaceState.ATI_ExitState;
                    break;
                }

                if (lowercaseInput.Length == 1 && char.IsDigit(lowercaseInput[0]) && 
                    char.GetNumericValue(lowercaseInput[0]) >= 1 && char.GetNumericValue( lowercaseInput[0]) <= 5)
                {
                    CurrentState = (ATInterfaceState)char.GetNumericValue(lowercaseInput[0]);
                }
            } while (CurrentState == ATInterfaceState.ATI_Mainmenu);
        }

        private void ShowAssets(List<Device> aAssets)
        {
             const int padding = 15;
            Console.Clear();
            LexiConsoleWriter.LexiWriteLine("AssetType".PadRight(padding) + "Office".PadRight(padding) + "Brand".PadRight(padding) + "Model".PadRight(padding) + "Purchase Date".PadRight(padding) + "Price in USD".PadRight(padding) + "Currency".PadRight(padding) + "Local price".PadRight(padding));
            LexiConsoleWriter.LexiWriteLine("---------".PadRight(padding) + "------".PadRight(padding) + "-----".PadRight(padding) + "-----".PadRight(padding) + "-------------".PadRight(padding) + "------------".PadRight(padding) + "--------".PadRight(padding) + "-----------".PadRight(padding));

            foreach (Device device in aAssets)
            {
                ShowAsset(device, padding);
                /*DateTime date = device.PurchaseDate.ToDateTime(new TimeOnly());
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
                Console.ResetColor();*/
            }
        }

        private void ShowAsset(Device aDevice, int aPadding)
        {
            DateTime date = aDevice.PurchaseDate.ToDateTime(new TimeOnly());
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

            Console.Write(aDevice.GetDeviceType().PadRight(aPadding));
            Console.Write(aDevice.OfficeLocation.PadRight(aPadding));
            Console.Write(aDevice.DeviceBrand.PadRight(aPadding));
            Console.Write(aDevice.ModelName.PadRight(aPadding));
            Console.Write(aDevice.PurchaseDate.ToString().PadRight(aPadding));
            Console.Write(aDevice.GetPriceInCurrency("USD").ToString(".##").PadRight(aPadding));
            Console.Write(aDevice.PurchasePrice.CurrencyCode.PadRight(aPadding));
            Console.Write(aDevice.PurchasePrice.Value.ToString(".##").PadRight(aPadding));
            Console.Write("\n");
            Console.ResetColor();
        }

        private int ShowVerticalSelection(List<string> someSelections, ConsoleKey aSelectionKey, int aStartIndex = 0)
        {
            int padding = 15;
            int currentLineSelection = aStartIndex;
            bool hasSelected = false;
            
            do
            {
                Console.Clear();
                currentLineSelection = int.Clamp(currentLineSelection, 1, AssetInfoFields.Length);

                for (int index = 0; index < someSelections.Count; index++)
                {
                    if (index == currentLineSelection)
                    {
                        Console.Write((">" + someSelections[index] + "<").PadRight(padding));
                    }
                    else
                    {
                        Console.Write((" " + someSelections[index] + " ").PadRight(padding));
                    }
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    currentLineSelection--;
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    currentLineSelection++;
                }
                else if (keyInfo.Key == aSelectionKey)
                {
                    hasSelected = true;
                }
                
            } while (!hasSelected);

            return currentLineSelection;
        }
        
        private void ShowAssetList()
        {
           //ShowAssets(dbContext.GetAllDevices());

           List<string> options = new List<string>
           {
               "Search by model",
               "Search by brand",
               "Search by office",
               "Search by purchase year",
               "Go back"
           };

           int selection = 0;
           do
           {
               Console.Clear();
               selection = ShowVerticalSelection(options, ConsoleKey.S);

               if (selection == options.Count - 1)
               {
                   CurrentState = ATInterfaceState.ATI_Mainmenu;
                   break;
               }
               else
               {
                   string userInput = "";
                   Console.Clear();
                   
                   LexiConsoleWriter.LexiWriteLine("Please write your search string");

                   ReadUserInput(out userInput);
               }
               
           } while (CurrentState == ATInterfaceState.ATI_ShowAssetList);
           

           // TODO: Add sorting...
           Console.ReadKey();
        }

        private static readonly string[] AssetInfoFields =
        {
            "Device Type",
            "SerialNumber",
            "DeviceBrand",
            "ModelName",
            "OfficeLocation",
            "Purchase Date",
            "Purchase Price"
        };
        
        private bool IsValidField(string someLowercaseFieldData, int aFieldIndex, out string aErrorString)
        {
            aErrorString = "";
            switch (aFieldIndex)
            {
                case 0: // Device Type
                    if (someLowercaseFieldData != "computer" && someLowercaseFieldData != "smartphone")
                    {
                        aErrorString = "Can only be 'computer' or 'smartphone'";
                        return false;
                    }

                    break;

                case 1: // SerialNumber
                    if (someLowercaseFieldData.Length != 13)
                    {
                        aErrorString = "Serial number MUST be of length 13 (input length: " +
                                       someLowercaseFieldData.Length + ")";
                        return false;
                    }

                    break;

                case 2: // DeviceBrand
                    break;

                case 3: // ModelName
                    break;

                case 4: // OfficeLocation
                    break;

                case 5: // PurchaseDate
                {
                    string[] data = someLowercaseFieldData.Split('-');

                    if (data.Length != 3)
                    {
                        aErrorString = "Must follow format: YYYY-MM-DD";
                        return false;
                    }

                    if (!DateTime.TryParse(someLowercaseFieldData, out var time))
                    {
                        aErrorString = "Failed to cast to date time! (follow format 'YYYY-MM-DD')";
                        return false;
                    }

                    break;
                }

                case 6: // PurchasePrice
                {
                    string[] data = someLowercaseFieldData.Split(' ');

                    if (data.Length != 2)
                    {
                        aErrorString = "Follow format '[AMOUNT] [CURRENCY CODE]'";
                        return false;
                    }

                    if (!int.TryParse(data[0], out var amresult))
                    {
                        aErrorString = "[AMOUNT] must be a number!";
                        return false;
                    }

                    if (!data[1].All(Char.IsLetter))
                    {
                        aErrorString = "[CURRENCY CODE] must be all letters (and a length of 3)!";
                        return false;
                    }
                    
                    if (data[1].Length != 3)
                    {
                        aErrorString = "[CURRENCY CODE] must be a length of 3! (USD, EUR, SEK etc...)";
                        return false;
                    }

                    break;
                }
            }

            return true;
        }

        private Device? CreateDeviceFromFieldArray(string[] aFieldArray)
        {
            Device? device = null;
            string[] priceSplit = aFieldArray.Last().Split(' ');
            Price price = new Price(decimal.Parse(priceSplit[0]), priceSplit[1].ToUpper());
            if (aFieldArray[0] == "computer")
            {
                device = new ComputerDevice(price, DateOnly.FromDateTime(DateTime.Parse(aFieldArray[5])),
                    aFieldArray[2], aFieldArray[3], aFieldArray[4], aFieldArray[1]);
            }
            else if (aFieldArray[0] == "smartphone")
            {
                device = new SmartphoneDevice(price, DateOnly.FromDateTime(DateTime.Parse(aFieldArray[5])),
                    aFieldArray[2], aFieldArray[3], aFieldArray[4], aFieldArray[1]);
            }

            return device;
        }
        
        private Device? AddAsset()
        {
            Device? device = null;

            bool isAddingAsset = true;

            List<string> deviceData = new List<string>();

            string userInput = "";
            string errorString = "";
            do
            {
                if (ReadUserInput(out userInput, AssetInfoFields[deviceData.Count] + ": ", true))
                {
                    break;
                }

                if (IsValidField(userInput, deviceData.Count, out errorString))
                {
                    deviceData.Add(userInput);
                }
                else
                {
                    LexiConsoleWriter.LexiWriteLine(errorString, ConsoleColor.Red);
                }
            } while (deviceData.Count < AssetInfoFields.Length);

            if (deviceData.Count == AssetInfoFields.Length)
            {

                device = CreateDeviceFromFieldArray(deviceData.ToArray());
                /*Price price = new Price(int.Parse(priceSplit[0]), priceSplit[1].ToUpper());
                if (deviceData[0] == "computer")
                {
                    device = new ComputerDevice(price, DateOnly.FromDateTime(DateTime.Parse(deviceData[5])),
                        deviceData[2], deviceData[3], deviceData[4], deviceData[1]);
                }
                else if (deviceData[0] == "smartphone")
                {
                    device = new SmartphoneDevice(price, DateOnly.FromDateTime(DateTime.Parse(deviceData[5])),
                        deviceData[2], deviceData[3], deviceData[4], deviceData[1]);
                }*/
            }

            return device;
        }

        private void ShowAddAsset()
        {
            string lowercaseInput = "";
            do
            {
                LexiConsoleWriter.LexiWriteLine("=== ADD ASSET ===");
                
                LexiConsoleWriter.LexiWriteLine("Type 'add' to add asset to database.");
                LexiConsoleWriter.LexiWriteLine("Type 'seed base' to seed base content to database.");

                LexiConsoleWriter.LexiWriteLine("Type back to go back.");

                if (ReadUserInput(out lowercaseInput))
                {
                    CurrentState = ATInterfaceState.ATI_Mainmenu;
                    break;
                }

                if (lowercaseInput == "add")
                {
                    Device? device = AddAsset();

                    if (device != null)
                    {
                        dbContext.AddDevice(device);
                    }
                }
                else if (lowercaseInput == "seed base")
                {
                    foreach (var dev in BaseDevices)
                    {
                        dbContext.AddDevice(dev);
                    }
                }
            } while (CurrentState == ATInterfaceState.ATI_AddAsset);
        }

        private void ShowDeleteAsset()
        {
            List<Device> devices = dbContext.GetAllDevices();
            int currentLineSelection = devices.Count;
            string lowercaseInput = "";
            do
            {
                Console.Clear();
                LexiConsoleWriter.LexiWriteLine("=== DELETE ASSET ===");
                LexiConsoleWriter.LexiWriteLine("Select asset to delete (Arrow up/down and 's' to select)");
                LexiConsoleWriter.LexiWriteLine("There is no confirmation, be careful!");

                currentLineSelection = int.Clamp(currentLineSelection, 0, devices.Count);
                
                for (int index = 0; index < devices.Count; index++)
                {
                    Device device = devices[index];
                    if (index == currentLineSelection)
                    {
                        Console.Write(" > ");
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                    
                    ShowAsset(device, 15);
                }

                if (currentLineSelection < devices.Count)
                {
                    Console.WriteLine("   Go back");
                }
                else
                {
                    Console.WriteLine(" > Go back");
                }
                
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    currentLineSelection--;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    currentLineSelection++;
                }
                else if (keyInfo.Key == ConsoleKey.S)
                {
                    if (currentLineSelection < devices.Count)
                    {
                        dbContext.DeleteDevice(devices[currentLineSelection]);
                        devices = dbContext.GetAllDevices();
                    }
                    else
                    {
                        CurrentState = ATInterfaceState.ATI_Mainmenu;
                        break;
                    }
                }
                
                /*if (ReadUserInput(out lowercaseInput))
                {
                    CurrentState = ATInterfaceState.ATI_Mainmenu;
                    break;
                }*/
                
            } while (CurrentState == ATInterfaceState.ATI_DeleteAsset);
        }

        private string[] GetAssetFieldsFromDevice(Device aDevice)
        {
            string[] foundFields = new string[AssetInfoFields.Length];
            
            /*
            "Device Type",
            "SerialNumber",
            "DeviceBrand",
            "ModelName",
            "OfficeLocation",
            "Purchase Date",
            "Purchase Price"
             */

            foundFields[0] = aDevice.GetDeviceType();
            foundFields[1] = aDevice.SerialNumber;
            foundFields[2] = aDevice.DeviceBrand;
            foundFields[3] = aDevice.ModelName;
            foundFields[4] = aDevice.OfficeLocation;
            foundFields[5] = aDevice.PurchaseDate.ToString();
            foundFields[6] = aDevice.PurchasePrice.Value + " " + aDevice.PurchasePrice.CurrencyCode;

            return foundFields;
        }

        private void UpdateDeviceFromAssetFieldArray(string[] aFieldArray, ref Device aDevice)
        {
            aDevice.SerialNumber = aFieldArray[1];
            aDevice.DeviceBrand = aFieldArray[2];
            aDevice.ModelName = aFieldArray[3];
            aDevice.OfficeLocation = aFieldArray[4];
            aDevice.PurchaseDate = DateOnly.FromDateTime(DateTime.Parse(aFieldArray[5])); //= aDevice.PurchaseDate.ToString();
            
            string[] priceSplit = aFieldArray.Last().Split(' ');
            Price price = new Price(decimal.Parse(priceSplit[0]), priceSplit[1].ToUpper());
            aDevice.PurchasePrice = price;
            
            //= aDevice.PurchasePrice.Value + " | " + aDevice.PurchasePrice.CurrencyCode;
            
        }
        
        private void UpdateAsset(Device aDevice)
        {
            string[] foundFields = GetAssetFieldsFromDevice(aDevice);
            bool isEditing = true;
            int padding = 15;
            int currentLineSelection = 1;
            do
            {
                Console.Clear();
                currentLineSelection = int.Clamp(currentLineSelection, 1, AssetInfoFields.Length);
                
                LexiConsoleWriter.LexiWriteLine("Updaing asset...");
                LexiConsoleWriter.LexiWriteLine("Use left and right arrows to navigate and enter to start editing.");
                LexiConsoleWriter.LexiWriteLine("Press 'q' to go back!");

                for (int index = 0; index < foundFields.Length; index++)
                {
                    if (index == currentLineSelection)
                    {
                        Console.Write((">" + foundFields[index] + "<").PadRight(padding));
                    }
                    else
                    {
                        Console.Write((" " + foundFields[index] + " ").PadRight(padding));
                    }
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    currentLineSelection--;
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    currentLineSelection++;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    LexiConsoleWriter.LexiWriteLine("Type 'back' to go back.");
                    LexiConsoleWriter.LexiWriteLine(("OLD: " + foundFields[currentLineSelection]), ConsoleColor.Gray);
                    string userInput = "";
                    string normalUserInput = "";
                    string fieldErrorString = "";
                    bool waitForInput = true;
                    do
                    {
                        if (ReadUserInput(out userInput, " > ", true ))
                        {
                            waitForInput = false;
                            break;
                        }

                        fieldErrorString = "---";
                        if (IsValidField(userInput, currentLineSelection, out fieldErrorString))
                        {
                            // NOTE: We can't modify the asset type (doesn't really make sense IMO) so we don't need to worry about change the entire class (both Device and DBDevice class)
                            // so we don't need more checks or code here.
                            foundFields[currentLineSelection] = userInput;
                            UpdateDeviceFromAssetFieldArray(foundFields, ref aDevice);
                            dbContext.UpdateDevice(aDevice);
                            waitForInput = false;
                        }
                        else
                        {
                            LexiConsoleWriter.LexiWriteLine(fieldErrorString, ConsoleColor.Red);
                        }
                        
                    } while (waitForInput);
                }
                else if (keyInfo.Key == ConsoleKey.Q)
                {
                    isEditing = false;
                }
                
            } while (isEditing);
        }
        private void ShowUpdateAsset()
        {
              List<Device> devices = dbContext.GetAllDevices();
            int currentLineSelection = devices.Count;
            string lowercaseInput = "";
            do
            {
                Console.Clear();
                LexiConsoleWriter.LexiWriteLine("=== UPDATE ASSET ===");
                LexiConsoleWriter.LexiWriteLine("Select asset to update (Arrow up/down and 's' to select)");

                currentLineSelection = int.Clamp(currentLineSelection, 0, devices.Count);
                
                for (int index = 0; index < devices.Count; index++)
                {
                    Device device = devices[index];
                    if (index == currentLineSelection)
                    {
                        Console.Write(" > ");
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                    
                    ShowAsset(device, 15);
                }

                if (currentLineSelection < devices.Count)
                {
                    Console.WriteLine("   Go back");
                }
                else
                {
                    Console.WriteLine(" > Go back");
                }
                
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    currentLineSelection--;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    currentLineSelection++;
                }
                else if (keyInfo.Key == ConsoleKey.S)
                {
                    if (currentLineSelection < devices.Count)
                    {
                        UpdateAsset(devices[currentLineSelection]);
                    }
                    else
                    {
                        CurrentState = ATInterfaceState.ATI_Mainmenu;
                        break;
                    }
                }
                
            } while (CurrentState == ATInterfaceState.ATI_UpdateAsset);
        }
    }
}
