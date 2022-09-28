using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DefaultNamespace.SaveSystem {
    public class SaveData {
        
        [JsonProperty("SaveDatas")]
        public List<BusinessSaveData> SaveDatas { get; }
        
        [JsonProperty("Balance")]
        public float Balance { get; }

        public SaveData(List<BusinessModel> businessModels, float balance) {

            Balance = balance;
            SaveDatas = new List<BusinessSaveData>();
            foreach (var model in businessModels) {
                SaveDatas.Add(new BusinessSaveData(model.Name, model.CurrentLvl, model.CurrentIncomeProgress, model.ImprovementsPurchased));
            }
        }

        public SaveData() {
            
        }
       
        [Serializable]
        public class BusinessSaveData {
            
            [JsonProperty("name")] 
            public string Name { get; }
        
            [JsonProperty("currentLvl")]
            public int CurrentLvl { get; }
        
            [JsonProperty("currentIncomeProgress")]
            public float CurrentIncomeProgress { get; }
        
            [JsonProperty("improvementsPurchased")]
            public List<bool> ImprovementsPurchased { get; }


            public BusinessSaveData(string name, int currentLvl, float currentIncomeProgress,
                List<bool> improvementsPurchased) {
                Name = name;
                CurrentLvl = currentLvl;
                CurrentIncomeProgress = currentIncomeProgress;
                ImprovementsPurchased = improvementsPurchased;
            }
        }
        
    }
}