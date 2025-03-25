using System;
using System.IO;
using System.Text.Json;

namespace PetCareGame
{
    public class SaveFile{
        public static bool BathDone { get; set; }
        public static bool NailTrimDone { get; set; }
        public static bool BrushingDone { get; set; }
        public static bool PetCareDone { get; set; }

        public static bool SlidingGameDone { get; set; }
        public static bool WheresWaldoDone { get; set; }

        private const string PATH = "stats.json";

        public void Save(SaveFile saved)
        {
            string serializedText = JsonSerializer.Serialize<SaveFile>(saved);
            File.WriteAllText(PATH, serializedText);
        }

        public SaveFile Load()
        {
            var fileContents = File.ReadAllText(PATH);
            return JsonSerializer.Deserialize<SaveFile>(fileContents);
        }
    }
}