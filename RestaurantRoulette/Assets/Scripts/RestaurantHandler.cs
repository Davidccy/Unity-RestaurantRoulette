using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class RestaurantHandler {
    #region Internal Fields
    private static List<RestaurantData> _rDataList = new List<RestaurantData>();
    private static RestaurantData _resultRData = null;

    private static UnityAction _dataChangedCallback;
    private static UnityAction _resultChangedCallback;
    #endregion

    #region Properties
    public static List<RestaurantData> RestaurantDataList {
        get {
            return _rDataList;
        }
    }

    public static int TotalWeight {
        get {
            int weight = 0;

            for (int i = 0; i < _rDataList.Count; i++) {
                weight += _rDataList[i].Weight;
            }

            return weight;
        }
    }

    public static RestaurantData RestaurantResultData {
        get {
            return _resultRData;
        }
    }
    #endregion

    #region APIs
    public static void RegisterCallback(UnityAction action) {
        _dataChangedCallback += action;
    }

    public static void UnregisterCallback(UnityAction action) {
        _dataChangedCallback -= action;
    }

    public static void RegisterResultCallback(UnityAction action) {
        _resultChangedCallback += action;
    }

    public static void UnregisterResultCallback(UnityAction action) {
        _resultChangedCallback -= action;
    }

    public static bool HasRestaurant(int index) {
        return index >= 0 && index < _rDataList.Count;
    }

    public static bool HasRestaurant(string name) {
        for (int i = 0; i < _rDataList.Count; i++) {
            if (_rDataList[i].Name == name) {
                return true;
            }
        }

        return false;
    }

    public static void AddNewRestaurant(RestaurantData rData) {
        _rDataList.Add(rData);

        _dataChangedCallback.Invoke();
    }

    public static void ModifyRestaurant(int index, RestaurantData newRData) {
        if (!HasRestaurant(index)) {
            Debug.LogErrorFormat("Unexpected restaurant index {0}", index);
            return;
        }

        _rDataList[index] = newRData;

        _dataChangedCallback.Invoke();
    }

    public static void RemoveRestaurant(RestaurantData rData) {
        _rDataList.Remove(rData);

        _dataChangedCallback.Invoke();
    }

    public static void RemoveRestaurant(int index) {
        _rDataList.RemoveAt(index);

        _dataChangedCallback.Invoke();
    }

    public static void ClearAllRestaurant() {
        _rDataList.Clear();

        _dataChangedCallback.Invoke();
    }

    public static void SetResultRestaurant(RestaurantData resultRData) {
        _resultRData = resultRData;

        _resultChangedCallback.Invoke();
    }
    #endregion
}
