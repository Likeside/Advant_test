using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace {
    public class BusinessView: MonoBehaviour {
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] Slider _incomeSlider;
        [SerializeField] TextMeshProUGUI _lvl;
        [SerializeField] TextMeshProUGUI _income;
        [SerializeField] TextMeshProUGUI _lvlUpCost;

        [SerializeField] Transform _improvementsRow;
        [SerializeField] GameObject _improvementPrefab;

        List<TextMeshProUGUI> _improvementTexts;
        
        public void SetBusiness(BusinessesConfig.Business business) {
            _name.text = business.name;
            _improvementTexts = new List<TextMeshProUGUI>();
            foreach (var improvement in business.improvements) {
                var imprGo = Instantiate(_improvementPrefab, _improvementsRow);
                var tmp = imprGo.GetComponentInChildren<TextMeshProUGUI>();
                tmp.text = improvement.cost + " \n" + improvement.incomeAmlifierInPercents;
               _improvementTexts.Add(tmp);
            }
        }

        public void UpdateBusiness(int currentLvl, float income, float lvlUpCost, List<bool> improvementsPurchased) {
            // TODO: ДОБАВИТЬ ТЕКСТЫ ПЕРЕД ЗНАЧЕНИЯМИ
            _lvl.text = currentLvl.ToString();
            _income.text = income.ToString(CultureInfo.InvariantCulture);
            _lvlUpCost.text = lvlUpCost.ToString(CultureInfo.InvariantCulture);

            for (int i = 0; i < _improvementTexts.Count; i++) {
                if (improvementsPurchased[i]) {
                    _improvementTexts[i].text = "Purch"; //убрать хардкод
                }
            }
        }

        public void UpdateSlider(float incomeProgress) {
            _incomeSlider.value = incomeProgress;
        }
        
    }
}