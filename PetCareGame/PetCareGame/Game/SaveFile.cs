using System;
using System.IO;
using System.Text.Json;

namespace PetCareGame
{
    public class SaveFile
    {
        public static bool BathDone { get; set; }
        public static bool NailTrimDone { get; set; }
        public static bool BrushingDone { get; set; }
        public static bool PetCareDone { get; set; }

        public static bool SlidingGameDone { get; set; }
        public static bool WheresWaldoDone { get; set; }

        private const string PATH = @"stats.json";

        public static void Save(SaveFile saved)
        {
            GameHandler.petCareLevel.SaveData(saved);
            //GameHandler.waldoLevel.SaveData(saved);
            //GameHandler.slidingLevel.SaveData(saved);
            string serializedText = JsonSerializer.Serialize<SaveFile>(saved);
            File.WriteAllText(PATH, serializedText);
        }

        public SaveFile Load()
        {
            var fileContents = File.ReadAllText(PATH);
            return JsonSerializer.Deserialize<SaveFile>(fileContents);
        }

        public static bool doesFileExist()
        {
            string curFile = @"stats.json";
            if (File.Exists(curFile))
            {
                return true;
            }
            return false;
        }

        public static void NewFile(SaveFile saved) {
            ResetVariables();
            string serializedText = JsonSerializer.Serialize<SaveFile>(saved);
            File.WriteAllText(PATH, serializedText);
        }

        private static void ResetVariables() {
            BathDone = false;
            NailTrimDone = false;
            BrushingDone = false;
            PetCareDone = false;
            SlidingGameDone = false;
            WheresWaldoDone = false;
        }
    }
}