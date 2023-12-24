using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWeightSettingMenu : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Button _btnAddnew = null;

    [SerializeField] private GameObject _goWeightObjectRoot = null;
    [SerializeField] private UIWeightSettingObject _uiObjectRes = null;
    #endregion

    #region Internal Fields
    private int _targetIndex;
    private List<UIWeightSettingObject> _uiWeightObjList = new List<UIWeightSettingObject>();
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btnAddnew.onClick.AddListener(ButtonAddNewOnClick);

        RestaurantHandler.RegisterDataChangedCallback(RestaurantDataChanged);
    }

    private void OnDestroy() {
        _btnAddnew.onClick.RemoveAllListeners();

        RestaurantHandler.UnregisterDataChangedCallback(RestaurantDataChanged);
    }
    #endregion

    #region UI Button Handlings
    private void ButtonAddNewOnClick() {
        RestaurantHandler.SendInputRequest(RestaurantHandler.RestaurantDataList.Count, string.Empty, 10);
    }
    #endregion

    #region Restaurant Data Handlings
    private void RestaurantDataChanged() {
        RefreshRestaurantList();
    }
    #endregion

    #region Internal Fields
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
    #endregion
}
