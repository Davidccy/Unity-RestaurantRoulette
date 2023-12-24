using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeightSettingMenu : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Button _btnAddnew = null;

    [SerializeField] private GameObject _goWeightObjectRoot = null;
    [SerializeField] private GameObject _goMask = null;

    [SerializeField] private UIWeightSettingObject _uiObjectRes = null;
    #endregion

    #region Internal Fields
    private List<UIWeightSettingObject> _uiWeightObjList = new List<UIWeightSettingObject>();
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        InitUI();

        _btnAddnew.onClick.AddListener(ButtonAddNewOnClick);

        RestaurantHandler.RegisterDataChangedCallback(RestaurantDataChanged);
        RestaurantHandler.RegisterStatusChangedCallback(RouletteStatusChanged);
    }

    private void OnDestroy() {
        _btnAddnew.onClick.RemoveAllListeners();

        RestaurantHandler.UnregisterDataChangedCallback(RestaurantDataChanged);
        RestaurantHandler.UnregisterStatusChangedCallback(RouletteStatusChanged);
    }
    #endregion

    #region UI Button Handlings
    private void ButtonAddNewOnClick() {
        RestaurantHandler.SendInputRequest(RestaurantHandler.RestaurantDataList.Count, string.Empty, 10);
    }
    #endregion

    #region Restaurant Handler Related Callbacks
    private void RestaurantDataChanged() {
        RefreshRestaurantList();
        CheckRestaurantAmount();
    }

    private void RouletteStatusChanged(RouletteStatus status) {
        _goMask.SetActive(status == RouletteStatus.Spinning);
    }
    #endregion

    #region Internal Fields
    private void InitUI() {
        _goMask.SetActive(false);
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
        RestaurantHandler.SendInputRequest(index, rData.Name, rData.Weight);
    }

    private void CheckRestaurantAmount() {
        _btnAddnew.interactable = _uiWeightObjList.Count < 20;
    }
    #endregion
}
