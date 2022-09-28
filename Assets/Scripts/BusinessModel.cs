using System;
using System.Collections.Generic;
using DefaultNamespace.SaveSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DefaultNamespace {
    
    public class BusinessModel {

        public event Action<float> OnSliderValueChangedEvent;
        public event Action<int, float, float, List<bool>> OnBusinessStatusUpdated;
        public event Action<float> OnIncomeReceivedEvent;

        BusinessesConfig.Business _business;
        public string Name => _business.name;
        public int CurrentLvl => _currentLvl;
        public float CurrentIncomeProgress => _currentIncomeProgress;
        
        public List<bool> ImprovementsPurchased => _improvementsPurchased;

        int _currentLvl;
        float _currentIncomeProgress;
        List<bool> _improvementsPurchased;

        public BusinessModel(BusinessesConfig.Business business, bool isDefaultBusiness = false, SaveData.BusinessSaveData saveData = null) {
            _business = business;
            if (saveData != null) {
                _currentLvl = saveData.CurrentLvl;
                _currentIncomeProgress = saveData.CurrentIncomeProgress;
                _improvementsPurchased = saveData.ImprovementsPurchased;
            }
            else {
                _currentLvl = isDefaultBusiness ? 1 : 0;
                _currentIncomeProgress = 0;
                _improvementsPurchased = new List<bool>();
                foreach (var improvement in _business.improvements) {
                    _improvementsPurchased.Add(false);
                }
            }
            
        }
        
        public bool LevelUp(float currentBalance) {
            if (GetLevelUpCost() <= currentBalance) {
                _currentLvl++;
                OnBusinessStatusUpdated?.Invoke(_currentLvl, GetIncome(), GetLevelUpCost(), ImprovementsPurchased );
                return true;
            }
            return false;
        }

        public bool PurchaseImprovement(int index) {
            if (index >= 0 && index < ImprovementsPurchased.Count) {
                ImprovementsPurchased[index] = true;
                OnBusinessStatusUpdated?.Invoke(_currentLvl, GetIncome(), GetLevelUpCost(), ImprovementsPurchased );
                return true;
            }
            return false;
        }
        
        public void UpdateSlider() {
            if (_currentIncomeProgress >= 1f) {
                _currentIncomeProgress = 0f;
                OnIncomeReceivedEvent?.Invoke(GetIncome());
            }
            else {
                float valueChange = Time.deltaTime / _business.incomeDelay;
                _currentIncomeProgress += valueChange;
            }
            OnSliderValueChangedEvent?.Invoke(_currentIncomeProgress);
        }

        float GetIncome() {
            float improvementsAmplifier = 1;
            foreach (var improvement in _business.improvements) {
                improvementsAmplifier += improvement.incomeAmlifierInPercents / 100;
            }
            return _currentLvl * _business.baseIncome * improvementsAmplifier;
        }

        float GetLevelUpCost() {
            return (_currentLvl + 1) * _business.baseCost;
        }
        
    }
}