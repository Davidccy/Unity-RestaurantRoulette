using System;
using System.Collections.Generic;
using UnityEngine;

public static class RestaurantHandler {
    #region Internal Fields
    private static List<RestaurantData> _rDataList = new List<RestaurantData>();
    private static RestaurantData _resultRData = null;

    private static Action _onDataChangedAction;
    private static Action _onResultChangedAction;
    private static Action<int, string, int> _onInputRequestAction;
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
    public static void RegisterDataChangedCallback(Action action) {
        _onDataChangedAction += action;
    }

    public static void UnregisterDataChangedCallback(Action action) {
        _onDataChangedAction -= action;
    }

    public static void RegisterResultChangedCallback(Action action) {
        _onResultChangedAction += action;
    }

    public static void UnregisterResultChangedCallback(Action action) {
        _onResultChangedAction -= action;
    }

    public static void RegisterInputRequestCallback(Action<int, string, int> action) {
        _onInputRequestAction += action;
    }

    public static void UnregisterInputRequestCallback(Action<int, string, int> action) {
        _onInputRequestAction -= action;
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

    public static RestaurantData GetRestaurant(int index) {
        if (index >= 0 && index < _rDataList.Count) {
            return _rDataList[index];
        }

        return null;
    }

    public static void SendInputRequest(int index, string defaultName, int defaultWeight) {
        _onInputRequestAction(index, defaultName, defaultWeight);
    }

    public static void AddNewRestaurant(RestaurantData rData) {
        _rDataList.Add(rData);

        _onDataChangedAction.Invoke();
    }

    public static void ModifyRestaurant(int index, RestaurantData newRData) {
        if (!HasRestaurant(index)) {
            Debug.LogErrorFormat("Unexpected restaurant index {0}", index);
            return;
        }

        _rDataList[index] = newRData;

        _onDataChangedAction.Invoke();
    }

    public static void RemoveRestaurant(RestaurantData rData) {
        _rDataList.Remove(rData);

        _onDataChangedAction.Invoke();
    }

    public static void RemoveRestaurant(int index) {
        _rDataList.RemoveAt(index);

        _onDataChangedAction.Invoke();
    }

    public static void ClearAllRestaurant() {
        _rDataList.Clear();

        _onDataChangedAction.Invoke();
    }

    public static void SetResultRestaurant(RestaurantData resultRData) {
        _resultRData = resultRData;

        _onResultChangedAction.Invoke();
    }
    #endregion
}
