using Lexicon_ConsoleWriter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Xml;

namespace Lexicon_IndividalProject1_InventoryManagement
{

    internal class LexiFileReader
    {
        private string CurrentDirectory = Directory.GetCurrentDirectory();
        private string CurrentFilename = "";
        private bool HasLoadedData = false;
        public string FileData { get; private set; } = "";

        public LexiFileReader(string aFilename)
        {
            CurrentFilename = aFilename;
            HasLoadedData = false;

            LoadOrCreate();
        }

        static public bool DoesFileExist(string aFilename)
        {
            return File.Exists(Directory.GetCurrentDirectory() + "\\" + aFilename);
        }

        private string GetCurrentFullPath() {  return CurrentDirectory + "\\" + CurrentFilename; }

        private string GetCurrentDirectory()
        {
            string fullPath = GetCurrentFullPath();
            string directory = fullPath.Substring(0, fullPath.LastIndexOf('\\'));

            return directory;
        }

        public bool LoadOrCreate()
        {
            bool Success = false;
            if (File.Exists(GetCurrentFullPath()))
            {
                Success = LoadFile();
            }
            else
            {
                Success = CreateFile();
            }

            HasLoadedData = Success;
            return Success;
        }

        public bool SaveData()
        {
            if (!HasLoadedData)
            {
                LexiConsoleWriter.LexiWriteLine("ERROR [SaveData]: Tried to save data", ConsoleColor.Red);
                return false;
            }

            bool Success = false;
            File.WriteAllText(GetCurrentFullPath(), FileData);

            return Success;
        }

        public void SetDataAsJSONObject<JSONObject>(JSONObject? aJSONObject, bool aShouldPrettyPrint = false)
        {
            List<JSONObject?> list = new List<JSONObject>();
            list.Add(aJSONObject);

            SetDataAsJSONObjectList<JSONObject>(list, aShouldPrettyPrint);
        }
        public void SetDataAsJSONObjectList<JSONObject>(List<JSONObject?>? aJSONList, bool aShouldPrettyPrint = false)
        {
            if (aJSONList == null)
            {
                LexiConsoleWriter.LexiWriteLine("ERROR [SetDataAsObjectList]: The list is NULL! NOT GOING TO SET DATA", ConsoleColor.Red);
                return;
            }

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = aShouldPrettyPrint;

            string jsonStringResult = JsonSerializer.Serialize(aJSONList, options);
            if (String.IsNullOrEmpty(jsonStringResult))
            {
                LexiConsoleWriter.LexiWriteLine("ERROR [SetDataAsObjectList]: The list is EMPTY! NOT GOING TO SET DATA", ConsoleColor.Red);
                return;
            }
           
            FileData = jsonStringResult;
        }
        public List<JSONObject?> GetDataAsJSONObjectList<JSONObject>()
        {
            List<JSONObject?>? output = new List<JSONObject?>();

           output = JsonSerializer.Deserialize<List<JSONObject?>>(FileData);

            return output != null ? output : new List<JSONObject?>();
        }

        public void SetDataAsString(string aString)
        {
            if (!HasLoadedData)
            {
                return;
            }

            FileData = aString;
        }

        private bool CreateFile(bool aShouldOverwrite = false)
        {
            if (File.Exists(CurrentFilename) && !aShouldOverwrite)
            {
                LexiConsoleWriter.LexiWriteLine($"ERROR [CreateFile]: Trying to create file '{CurrentFilename}'. That file exists but we shouldn't overwrite it!", ConsoleColor.Red);
                return false;
            }

            try
            {
                if (!Directory.Exists(GetCurrentDirectory()))
                {
                    Directory.CreateDirectory(GetCurrentDirectory());
                }

                FileStream stream = File.Create(GetCurrentFullPath());
                FileData = "";

                stream.Close();
            }
            catch (Exception error) 
            {
                LexiConsoleWriter.LexiWriteLine($"ERROR [CreateFile]: Tried to create '{CurrentFilename}', but there was an exception!", ConsoleColor.Red);
                LexiConsoleWriter.LexiWriteLine(error.Message, ConsoleColor.Red);
            }

            return true;
        }

        private bool LoadFile()
        {
            if (!File.Exists(CurrentFilename))
            {
                LexiConsoleWriter.LexiWriteLine($"ERROR [LoadFile]: File with name '{CurrentFilename}' doesn't exist.", ConsoleColor.Red);
                return false;
            }

            try
            {
                FileData = File.ReadAllText(GetCurrentFullPath());
            }
            catch (Exception error)
            {
                LexiConsoleWriter.LexiWriteLine($"ERROR [LoadFile]: Failed to load file with name '{CurrentFilename}'", ConsoleColor.Red);
                LexiConsoleWriter.LexiWriteLine(error.Message, ConsoleColor.Red);
                return false;
            }

            return true;
        }
    }
}
