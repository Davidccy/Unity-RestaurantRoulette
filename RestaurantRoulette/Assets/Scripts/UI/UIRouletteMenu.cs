using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRouletteMenu : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private GameObject _goDecoration = null;
    [SerializeField] private GameObject _goArrow = null;
    [SerializeField] private Button _btnSpin = null;
    [SerializeField] private Button _btnStop = null;

    [SerializeField] private GameObject _goRouletteObjectRoot = null;
    [SerializeField] private UIRouletteObject _uiObjectRes = null;

    [SerializeField] private List<Color> _colorList = null;
    [SerializeField] private Color _colorSpecial = Color.black;

    [Header("Spin Settings")]
    [SerializeField] private float _speedUpBuffTime = 2.0f;
    [SerializeField] private float _spinTime = 5.0f;
    [SerializeField] private float _slowDownBuffTime = 2.0f;
    [SerializeField] private float _maxSpinSpeed = 20; // Z angle
    [SerializeField] private bool _isClockWise = false;    
    #endregion

    #region Internal Fields
    private bool _isSpinning;
    private RectTransform _rectRouletteObjectRoot;
    private int _resultIndex = 0;
    private List<UIRouletteObject> _uiRouletteObjList = new List<UIRouletteObject>();
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        InitUI();

        _btnSpin.onClick.AddListener(ButtonSpinOnClick);
        _btnStop.onClick.AddListener(ButtonStopOnClick);

        RestaurantHandler.RegisterCallback(RestaurantDataChanged);
    }

    private void OnDestroy() {
        _btnSpin.onClick.RemoveAllListeners();
        _btnStop.onClick.RemoveAllListeners();

        RestaurantHandler.UnregisterCallback(RestaurantDataChanged);
    }
    #endregion

    #region UI Button Handlings
    private void ButtonSpinOnClick() {
        RestaurantHandler.SetResultRestaurant(null);
        _uiRouletteObjList[_resultIndex].StopAnimation();

        StartCoroutine(CoStartSpin());

        _btnSpin.gameObject.SetActive(false);
        _btnStop.gameObject.SetActive(true);
    }

    private void ButtonStopOnClick() {
        RestaurantHandler.SetResultRestaurant(null);

        _isSpinning = false;

        _btnSpin.gameObject.SetActive(true);
        _btnStop.gameObject.SetActive(false);
    }
    #endregion

    #region Restaurant Data Handlings
    private void RestaurantDataChanged() {
        RefreshRoulette();
    }
    #endregion

    #region Coroutines
    private IEnumerator CoStartSpin() {
        if (_isSpinning) {
            yield break;
        }

        _isSpinning = true;
        float passedTime = 0;
        float spinSpeed = 0;

        float angleZ = _rectRouletteObjectRoot.localEulerAngles.z;
        while (_isSpinning) {
            spinSpeed = GetSpinSpeed(passedTime);
            spinSpeed = _isClockWise ? -spinSpeed : spinSpeed;

            angleZ += spinSpeed;

            _rectRouletteObjectRoot.rotation = Quaternion.Euler(0, 0, angleZ);

            yield return new WaitForEndOfFrame();

            passedTime += Time.deltaTime;

            if (passedTime > _speedUpBuffTime + _spinTime + _slowDownBuffTime) {
                _isSpinning = false;
            }
        }

        SpinFinished();
    }
    #endregion

    #region Internal Fields
    private void InitUI() {
        _goDecoration.SetActive(false);
        _goArrow.SetActive(false);

        _btnSpin.gameObject.SetActive(false);
        _btnSpin.interactable = false;

        _btnStop.gameObject.SetActive(false);
        _btnStop.interactable = false;

        _rectRouletteObjectRoot = _goRouletteObjectRoot.transform as RectTransform;
    }

    private void RefreshRoulette() {
        List<RestaurantData> restaurantDataList = RestaurantHandler.RestaurantDataList;

        _goDecoration.SetActive(restaurantDataList.Count > 0);
        _goArrow.SetActive(restaurantDataList.Count > 1);

        _btnSpin.gameObject.SetActive(restaurantDataList.Count > 1);
        _btnSpin.interactable = restaurantDataList.Count > 1;

        _btnStop.gameObject.SetActive(false);

        int totalWeight = RestaurantHandler.TotalWeight;

        int accumulativeWeight = 0;
        float percentageWeight = 0;
        float percentageAccumulativeWeight = 0;
        float angle = 0;

        for (int i = 0; i < restaurantDataList.Count; i++) {
            if (i >= _uiRouletteObjList.Count) {
                UIRouletteObject newObj = Instantiate(_uiObjectRes, _goRouletteObjectRoot.transform);
                _uiRouletteObjList.Add(newObj);
            }

            UIRouletteObject obj = _uiRouletteObjList[i];
            RestaurantData rData = restaurantDataList[i];

            percentageWeight = (float) rData.Weight / totalWeight;
            percentageAccumulativeWeight = (float) accumulativeWeight / totalWeight;
            angle = 0 - percentageAccumulativeWeight * 360;

            obj.SetName(rData.Name);
            obj.SetColor(GetColor(i, i == restaurantDataList.Count - 1));
            obj.SetWeight(rData.Weight, totalWeight);
            obj.SetAngle(angle);
            obj.gameObject.SetActive(true);

            accumulativeWeight += rData.Weight;
        }

        for (int i = restaurantDataList.Count; i < _uiRouletteObjList.Count; i++) {
            _uiRouletteObjList[i].gameObject.SetActive(false);
        }
    }

    private Color GetColor(int index, bool isFinal) {
        if (_colorList == null || _colorList.Count == 0) {
            return Color.white;
        }

        int totalColorCount = _colorList.Count;
        int colorIndex = index % totalColorCount;
        Color c = _colorList[colorIndex];
        
        if (index != 0 && isFinal && colorIndex == 0) {
            return _colorSpecial;
        }

        return c;
    }
    
    private void CalculateResult() {
        float resultAngleZ = _rectRouletteObjectRoot.localRotation.eulerAngles.z + 90;
        if (resultAngleZ < 0) {
            resultAngleZ += 360;
        }
        else if (resultAngleZ > 360) {
            resultAngleZ -= 360;
        }
        float resultRatio = resultAngleZ / 360;

        int resultIndex = -1;        
        int totlaWeight = RestaurantHandler.TotalWeight;
        int weightRangeMin = 0;
        int weightRangeMax = 0;
        int accumulativeWeight = 0;

        for (int i = 0; i < RestaurantHandler.RestaurantDataList.Count; i++) {
            RestaurantData rData = RestaurantHandler.RestaurantDataList[i];

            weightRangeMin = accumulativeWeight;
            weightRangeMax += rData.Weight;

            float ratioMin = (float) weightRangeMin / totlaWeight;
            float ratioMax = (float) weightRangeMax / totlaWeight;

            if (resultRatio >= ratioMin && ratioMax > resultRatio) {
                resultIndex = i;
                break;
            }

            accumulativeWeight += rData.Weight;
        }

        if (resultIndex == -1) {
            Debug.LogErrorFormat("Unexpected result {0}", resultIndex);
            resultIndex = 0;
        }

        _resultIndex = resultIndex;
        RestaurantHandler.SetResultRestaurant(RestaurantHandler.RestaurantDataList[_resultIndex]);
    }

    private void SpinFinished() {
        _isSpinning = false;

        _btnSpin.gameObject.SetActive(true);
        _btnStop.gameObject.SetActive(false);

        CalculateResult();
        ShowAnimation();
    }

    private void ShowAnimation() {
        _uiRouletteObjList[_resultIndex].PlayAnimation();
    }

    private void StopAnimation() {
        _uiRouletteObjList[_resultIndex].StopAnimation();
    }

    private float GetSpinSpeed(float passedTime) {
        float spinSpeed = 0;

        if (passedTime < _speedUpBuffTime) {
            spinSpeed = Mathf.Clamp01(passedTime / _speedUpBuffTime) * _maxSpinSpeed;
        }
        else if (passedTime >= _speedUpBuffTime && passedTime <= _speedUpBuffTime + _spinTime) {
            spinSpeed = _maxSpinSpeed;
        }
        else if (passedTime > _speedUpBuffTime + _spinTime) {
            float temp = passedTime - (_speedUpBuffTime + _spinTime);
            spinSpeed = (1 - Mathf.Clamp01(temp / _slowDownBuffTime)) * _maxSpinSpeed;
        }

        return spinSpeed;
    }
    #endregion
}
