using System;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace PetCareGame
{
    public class SaveFile
    {
        public bool BathDone { get; set; }
        public bool NailTrimDone { get; set; }
        public bool BrushingDone { get; set; }
        public bool PetCareDone { get; set; }
        public int catPosX { get; set; }
        public int catPosY { get; set; }

        public bool SlidingGameDone { get; set; }
        public bool WheresWaldoDone { get; set; }

        private const string PATH = @"stats.json";

        public void Save(SaveFile saved)
        {
            GameHandler.petCareLevel.SaveData(saved);
            //waldo level handles its saving differently
            GameHandler.slidingLevel.SaveData(saved);
            GameHandler.overworldLevel.SaveData(saved);
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

        public void NewFile(SaveFile saved) {
            ResetVariables();
            string serializedText = JsonSerializer.Serialize<SaveFile>(saved);
            File.WriteAllText(PATH, serializedText);
        }

        private void ResetVariables() {
            BathDone = false;
            NailTrimDone = false;
            BrushingDone = false;
            PetCareDone = false;
            SlidingGameDone = false;
            WheresWaldoDone = false;
            catPosX = 0;
            catPosY = 192;
        }
    }
}