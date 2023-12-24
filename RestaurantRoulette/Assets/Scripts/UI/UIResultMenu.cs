using TMPro;
using UnityEngine;

public class UIResultMenu : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _textName = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        InitUI();

        RestaurantHandler.RegisterResultChangedCallback(RestaurantResultDataChanged);
    }

    private void OnDestroy() {
        RestaurantHandler.UnregisterResultChangedCallback(RestaurantResultDataChanged);
    }
    #endregion

    #region Restaurant Data Handlings
    private void RestaurantResultDataChanged() {
        RefreshResult();
    }
    #endregion

    #region Internal Methods
    private void InitUI() {
        _textName.text = string.Empty;
    }

    private void RefreshResult() {
        RestaurantData resultRData = RestaurantHandler.RestaurantResultData;
        _textName.text = resultRData != null ? resultRData.Name : string.Empty;
    }
    #endregion
}
