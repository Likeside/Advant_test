using System.Collections.Generic;
using System.Globalization;
using DefaultNamespace.SaveSystem;
using TMPro;
using UnityEngine;

namespace DefaultNamespace {
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
            for (int i = 0; i < _businessModels.Count; i++) {
                _businessModels[i].OnBusinessStatusUpdated += _businessViews[i].UpdateBusiness;
                _businessModels[i].OnSliderValueChangedEvent += _businessViews[i].UpdateSlider;
                _businessModels[i].OnIncomeReceivedEvent += GetIncome; //  TODO: ОТПИСАТЬСЯ
            }
        }

        void Update() {
            foreach (var model in _businessModels) {
                model.UpdateSlider();
            }
        }
        void GetIncome(float income) {
            _balance += income;
            _balanceText.text = _balance.ToString(CultureInfo.InvariantCulture);
        }
        void CreateBusinesses() {
            _businessModels = new List<BusinessModel>();
            _businessViews = new List<BusinessView>();
            SaveData saveData = SaveSystem.SaveSystem.Load(out bool success);
            bool gameLoaded = false;
            if (success) {
                if(saveData == null) Debug.Log("sd null");
                if(saveData.SaveDatas == null) Debug.Log("sds null");
                if (saveData.SaveDatas.Count == _businessesConfig.businesses.Count) {
                    gameLoaded = true;
                    _balance = saveData.Balance;
                }
            }
            else {
                _balance = _businessesConfig.defaultBalance;
            }
            _balanceText.text = _balance.ToString(CultureInfo.InvariantCulture);
            for (int i = 0; i < _businessesConfig.businesses.Count; i++) {
                var businessModel = gameLoaded ? new BusinessModel(_businessesConfig.businesses[i], i == 0, saveData.SaveDatas[i]) 
                    : new BusinessModel(_businessesConfig.businesses[i], i == 0);
                _businessModels.Add(businessModel);
                var businessView = Instantiate(_businessView, _content).GetComponent<BusinessView>();
                businessView.SetBusiness(_businessesConfig.businesses[i]);
                _businessViews.Add(businessView);
            }
        }
    }
}