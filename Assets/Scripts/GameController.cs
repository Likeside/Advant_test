using System;
using System.Collections.Generic;
using System.Globalization;
using Scripts.SaveSystem;
using TMPro;
using UnityEngine;

namespace Scripts {
    public class GameController: MonoBehaviour {
        [SerializeField] BusinessesConfig _businessesConfig;
        [SerializeField] Transform _content;
        [SerializeField] GameObject _businessView;
        [SerializeField] TextMeshProUGUI _balanceText;
        
        List<BusinessModel> _businessModels;
        List<BusinessView> _businessViews;
        float _balance;
        
        void Start() {
            CreateBusinesses();
            Subscribe(); //  TODO: ОТПИСАТЬСЯ
        }
        void Update() {
            foreach (var model in _businessModels) {
                model.UpdateSlider();
            }
            SaveSystem.SaveSystem.Save(_businessModels, _balance);
        }
        
        void SpendMoney(float cost) {
            _balance -= cost;
            if (_balance < 0) {
                _balance = 0;
            }
            _balanceText.text = _balance.ToString(CultureInfo.InvariantCulture)  + " $";;
        }
        
        void GetIncome(float income) {
            _balance += income;
            _balanceText.text = _balance.ToString(CultureInfo.InvariantCulture)  + " $";;
        }
        
        void CreateBusinesses() {
            _businessModels = new List<BusinessModel>();
            _businessViews = new List<BusinessView>();
            SaveData saveData = SaveSystem.SaveSystem.Load(out bool success);
            bool gameLoaded = false;
            if (success) {
                if (saveData.SaveDatas.Count == _businessesConfig.businesses.Count) {
                    gameLoaded = true;
                    for (int i = 0; i < saveData.SaveDatas.Count; i++) {
                        _balance = saveData.Balance;
                        //если в конфиге и в сохраненных данных для какого-либо бизнеса не совпадает количество возможных улучшений, грузим новую игру
                        if (saveData.SaveDatas[i].ImprovementsPurchased.Count !=
                            _businessesConfig.businesses[i].improvements.Count) {
                            Debug.Log("Improvement count changed for business at index: " + i + ", save file discarded");
                            gameLoaded = false;
                            _balance = _businessesConfig.defaultBalance;
                        }
                    }
                }
            }
            else {
                _balance = _businessesConfig.defaultBalance;
            }
            _balanceText.text = _balance.ToString(CultureInfo.InvariantCulture)  + " $";;
            for (int i = 0; i < _businessesConfig.businesses.Count; i++) {
                var businessModel = gameLoaded ? new BusinessModel(_businessesConfig.businesses[i], i == 0, saveData.SaveDatas[i]) 
                    : new BusinessModel(_businessesConfig.businesses[i], i == 0);
                _businessModels.Add(businessModel);
                var businessView = Instantiate(_businessView, _content).GetComponent<BusinessView>();
                businessView.SetBusiness(_businessesConfig.businesses[i]);
                _businessViews.Add(businessView);
            }
        }

        void Subscribe() {
            for (int i = 0; i < _businessModels.Count; i++) {
                _businessModels[i].OnBusinessStatusUpdatedEvent += _businessViews[i].UpdateBusiness;
                _businessModels[i].OnSliderValueChangedEvent += _businessViews[i].UpdateSlider;
                _businessModels[i].OnIncomeReceivedEvent += GetIncome; 
                var i1 = i;
                _businessViews[i].LevelUpButton.onClick.AddListener((() => _businessModels[i1].LevelUp(_balance)));

                for (int j = 0; j < _businessViews[i].ImprovementButtons.Count; j++) {
                    var j1 = j;
                    _businessViews[i].ImprovementButtons[j].onClick.AddListener((() => _businessModels[i1].PurchaseImprovement(j1, _balance)));
                }
                _businessModels[i].OnMoneySpentEvent += SpendMoney;
                if (_businessModels[i].CurrentLvl == 0) {
                    _businessModels[i].SetSliderToZero();
                }
                _businessModels[i].UpdateStatus();
            }
        }
        
        void Unsubscribe() {
            for (int i = 0; i < _businessModels.Count; i++) {
                _businessModels[i].OnBusinessStatusUpdatedEvent -= _businessViews[i].UpdateBusiness;
                _businessModels[i].OnSliderValueChangedEvent -= _businessViews[i].UpdateSlider;
                _businessModels[i].OnIncomeReceivedEvent -= GetIncome; 
                var i1 = i;
                _businessViews[i].LevelUpButton.onClick.RemoveAllListeners();

                for (int j = 0; j < _businessViews[i].ImprovementButtons.Count; j++) {
                    var j1 = j;
                    _businessViews[i].ImprovementButtons[j].onClick.RemoveAllListeners();
                }
                _businessModels[i].OnMoneySpentEvent -= SpendMoney;
            }
        }

        void OnApplicationFocus(bool hasFocus) {
            if(!hasFocus) SaveSystem.SaveSystem.Save(_businessModels, _balance);
        }

        void OnApplicationQuit() {
            Unsubscribe();
        }
    }
}