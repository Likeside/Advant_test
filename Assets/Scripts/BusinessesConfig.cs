using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BusinessConfig", menuName = "Configs/BusinessConfig", order = 1)]
public class BusinessesConfig : ScriptableObject {

    public float defaultBalance;
    public List<Business> businesses;
    
    [Serializable]
    public class Business {
        public string name;
        public float incomeDelay;
        public float baseCost;
        public float baseIncome;

        public List<Improvement> improvements;
    }
   
    
    [Serializable]
    public class Improvement {
        public string name;
        public float cost;
        public float incomeAmplifierInPercents;
    }
}
