using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts {
    public class BusinessView: MonoBehaviour {
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] Slider _incomeSlider;
        [SerializeField] TextMeshProUGUI _lvl;
        [SerializeField] TextMeshProUGUI _income;
        [SerializeField] TextMeshProUGUI _lvlUpCost;
        [SerializeField] Button _levelUpBtn;
        [SerializeField] Transform _improvementsRow;
        [SerializeField] GameObject _improvementPrefab;
        
        public Button LevelUpButton => _levelUpBtn;
        public List<Button> ImprovementButtons { get; private set; }

        List<TextMeshProUGUI> _improvementTexts;
        
        public void SetBusiness(BusinessesConfig.Business business) {
            _name.text = business.name;
            _improvementTexts = new List<TextMeshProUGUI>();
            ImprovementButtons = new List<Button>();
            for (int i = 0; i < business.improvements.Count; i++) {
                var improvement = business.improvements[i];
                var imprGo = Instantiate(_improvementPrefab, _improvementsRow);
                var tmp = imprGo.GetComponentInChildren<TextMeshProUGUI>();
                //захардкожено, т.к. вне тестового текст грузился бы из JSON (или другого текстового файла)
                tmp.text = $"{improvement.name}\nДоход: +{improvement.incomeAmplifierInPercents}%\nЦена: {improvement.cost}"; 
                _improvementTexts.Add(tmp);
               var btn = imprGo.GetComponent<Button>();
               ImprovementButtons.Add(btn);
            }
        }

        public void UpdateBusiness(int currentLvl, float income, float lvlUpCost, List<bool> improvementsPurchased) {
            _lvl.text = currentLvl.ToString();
            _income.text = income.ToString(CultureInfo.InvariantCulture) + " $";
            _lvlUpCost.text = lvlUpCost.ToString(CultureInfo.InvariantCulture)  + " $";
            for (int i = 0; i < _improvementTexts.Count; i++) {
                if (improvementsPurchased[i]) {
                    _improvementTexts[i].text = "Куплено"; 
                }
            }
        }

        public void UpdateSlider(float incomeProgress) {
            _incomeSlider.value = incomeProgress;
        }
    }
}