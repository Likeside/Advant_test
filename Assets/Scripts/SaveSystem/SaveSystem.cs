using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Scripts.SaveSystem {
    public class SaveSystem {

        static string _path = Application.persistentDataPath + "/saveddata.txt";
        
        public static SaveData Load(out bool success) {
            if (File.Exists(_path)) {
                string jsonString = File.ReadAllText(_path);
                SaveData saveData = JsonConvert.DeserializeObject<SaveData>(jsonString);
                success = true;
                return saveData;
            }
            success = false;
            return null;
        }
        
        public static void Save(List<BusinessModel> models, float balance) {
            var saveData = new SaveData(models, balance);
            string jsonString = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(_path, jsonString);
        }
    }
}