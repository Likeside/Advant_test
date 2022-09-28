using System;
using System.Collections.Generic;
using Scripts.SaveSystem;
using UnityEngine;

namespace Scripts {
    
    public class BusinessModel {
        public event Action<int, float, float, List<bool>> OnBusinessStatusUpdatedEvent;
        public event Action<float> OnSliderValueChangedEvent;
        public event Action<float> OnIncomeReceivedEvent;
        public event Action<float> OnMoneySpentEvent;

        public string Name => _business.name;
        public int CurrentLvl => _currentLvl;
        public float CurrentIncomeProgress => _currentIncomeProgress;
        public List<bool> ImprovementsPurchased => _improvementsPurchased;

        int _currentLvl;
        float _currentIncomeProgress;
        readonly BusinessesConfig.Business _business;
        readonly List<bool> _improvementsPurchased;

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
                for (int i = 0; i < _business.improvements.Count; i++) {
                    _improvementsPurchased.Add(false);
                }
            }
        }

        public void UpdateStatus() {
            OnBusinessStatusUpdatedEvent?.Invoke(_currentLvl, GetIncome(), GetLevelUpCost(), ImprovementsPurchased );
        }

        public void SetSliderToZero() {
            OnSliderValueChangedEvent?.Invoke(0f);
        }
        
        public void LevelUp(float currentBalance) {
            if (GetLevelUpCost() <= currentBalance) {
                _currentLvl++;
                OnBusinessStatusUpdatedEvent?.Invoke(_currentLvl, GetIncome(), GetLevelUpCost(), ImprovementsPurchased );
                OnMoneySpentEvent?.Invoke(GetLevelUpCost());
            }
            else {
             OnMoneySpentEvent?.Invoke(0);   
            }
        }

        public void PurchaseImprovement(int index, float currentBalance) {
            float cost = 0;
            if (currentBalance >= _business.improvements[index].cost && _currentLvl > 0) {
                if (index >= 0 && index < ImprovementsPurchased.Count) {
                    ImprovementsPurchased[index] = true;
                    OnBusinessStatusUpdatedEvent?.Invoke(_currentLvl, GetIncome(), GetLevelUpCost(),
                        ImprovementsPurchased);
                    cost = _business.improvements[index].cost;
                }
            }
            OnMoneySpentEvent?.Invoke(cost);
        }
        
        public void UpdateSlider() {
            if(_currentLvl <= 0) return;
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
            for (int i = 0; i < _improvementsPurchased.Count; i++) {
                if (_improvementsPurchased[i]) {
                 improvementsAmplifier += _business.improvements[i].incomeAmplifierInPercents / 100;
                }
            }
            if (_currentLvl == 0) return _business.baseIncome;
            return _currentLvl * _business.baseIncome * improvementsAmplifier;
        }

        float GetLevelUpCost() {
            return (_currentLvl + 1) * _business.baseCost;
        }
        
    }
}