using System;
using System.IO;
using System.Text.Json;

namespace PetCareGame
{
    public class SaveFile
    {
        public bool BathDone { get; set; }
        public bool NailTrimDone { get; set; }
        public bool BrushingDone { get; set; }
        public bool PetCareDone { get; set; }

        public bool SlidingGameDone { get; set; }
        public bool WheresWaldoDone { get; set; }

        private const string PATH = @"stats.json";

        public static void Save(SaveFile saved)
        {
            GameHandler.petCareLevel.SaveData(saved);
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
    }
}