using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWeightSettingMenu : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Button _btnAddnew = null;

    [SerializeField] private GameObject _goWeightObjectRoot = null;
    [SerializeField] private UIWeightSettingObject _uiObjectRes = null;

    [SerializeField] private GameObject _goInput = null;
    [SerializeField] private TMP_InputField  _inputName = null;
    [SerializeField] private TMP_InputField _inputWeight = null;
    [SerializeField] private Button _btnConfirm = null;
    [SerializeField] private Button _btnCancel = null;
    #endregion

    #region Internal Fields
    private int _targetIndex;
    private List<UIWeightSettingObject> _uiWeightObjList = new List<UIWeightSettingObject>();
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _goInput.SetActive(false);

        _btnAddnew.onClick.AddListener(ButtonAddNewOnClick);
        _btnConfirm.onClick.AddListener(ButtonConfirmOnClick);
        _btnCancel.onClick.AddListener(ButtonCancleOnClick);

        RestaurantHandler.RegisterCallback(RestaurantDataChanged);
    }

    private void OnDestroy() {
        _btnAddnew.onClick.RemoveAllListeners();
        _btnConfirm.onClick.RemoveAllListeners();
        _btnCancel.onClick.RemoveAllListeners();

        RestaurantHandler.UnregisterCallback(RestaurantDataChanged);
    }
    #endregion

    #region UI Button Handlings
    private void ButtonAddNewOnClick() {
        ShowInputMenu(RestaurantHandler.RestaurantDataList.Count, string.Empty, 10);
    }

    private void ButtonConfirmOnClick() {
        bool isModify = RestaurantHandler.HasRestaurant(_targetIndex);

        // Name
        string inputName = _inputName.text;
        if (inputName == string.Empty) {
            return;
        }
        else if (isModify) {
            string oriNmae = RestaurantHandler.RestaurantDataList[_targetIndex].Name;
            if (oriNmae == inputName) { 
                // Pass
            }
            else if (RestaurantHandler.HasRestaurant(inputName)) {
                return;
            }
        }
        else {
            if (RestaurantHandler.HasRestaurant(inputName)) {
                return;
            }
        }        

        // Weight
        string inputWeightStr = _inputWeight.text;
        if (!int.TryParse(inputWeightStr, out int inputWeight)) {
            return;
        }

        RestaurantData newRData = new RestaurantData { Name = inputName, Weight = inputWeight };
        if (isModify) {
            RestaurantHandler.ModifyRestaurant(_targetIndex, newRData);
        }
        else {
            RestaurantHandler.AddNewRestaurant(newRData);
        }

        _goInput.SetActive(false);
    }

    private void ButtonCancleOnClick() {
        _goInput.SetActive(false);
    }
    #endregion

    #region Restaurant Data Handlings
    private void RestaurantDataChanged() {
        RefreshRestaurantList();
    }
    #endregion

    #region Internal Fields
    private void ShowInputMenu(int index, string defaultName, int defaultWeight) {
        _goInput.SetActive(true);

        _targetIndex = index;
        _inputName.text = defaultName;
        _inputWeight.text = defaultWeight.ToString();
    }

    private void RefreshRestaurantList() {
        List<RestaurantData> restaurantDataList = RestaurantHandler.RestaurantDataList;

        for (int i = 0; i < restaurantDataList.Count; i++) {
            if (i >= _uiWeightObjList.Count) {
                UIWeightSettingObject newObj = Instantiate(_uiObjectRes, _goWeightObjectRoot.transform);
                _uiWeightObjList.Add(newObj);
            }

            UIWeightSettingObject obj = _uiWeightObjList[i];
            RestaurantData rData = restaurantDataList[i];

            obj.SetModifyCallback(ModifyCallback);
            obj.SetIndex(i);
            obj.SetName(rData.Name);
            obj.SetWeight(rData.Weight);
            obj.gameObject.SetActive(true);
        }

        for (int i = restaurantDataList.Count; i < _uiWeightObjList.Count; i++) {
            _uiWeightObjList[i].gameObject.SetActive(false);
        }
    }

    private void ModifyCallback(int index) {
        RestaurantData rData = RestaurantHandler.RestaurantDataList[index];
        ShowInputMenu(index, rData.Name, rData.Weight);
    }
    #endregion
}
