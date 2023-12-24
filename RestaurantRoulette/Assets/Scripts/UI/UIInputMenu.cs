using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInputMenu : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private GameObject _goRoot = null;

    [SerializeField] private TMP_InputField _inputName = null;
    [SerializeField] private TMP_InputField _inputWeight = null;
    [SerializeField] private Button _btnConfirm = null;
    [SerializeField] private Button _btnCancel = null;

    [SerializeField] private TextMeshProUGUI _textWarningMsg = null;
    #endregion

    #region Internal Fields
    private int _targetIndex;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        InitUI();        

        _inputName.onValueChanged.AddListener(InputNameChanged);
        _inputWeight.onValueChanged.AddListener(InputWeightChanged);

        _btnConfirm.onClick.AddListener(ButtonConfirmOnClick);
        _btnCancel.onClick.AddListener(ButtonCancleOnClick);

        RestaurantHandler.RegisterInputRequestCallback(RestaurantDataChanged);
    }

    private void OnDestroy() {
        _btnConfirm.onClick.RemoveAllListeners();
        _btnCancel.onClick.RemoveAllListeners();

        _inputName.onValueChanged.RemoveAllListeners();
        _inputWeight.onValueChanged.RemoveAllListeners();

        RestaurantHandler.UnregisterInputRequestCallback(RestaurantDataChanged);
    }
    #endregion

    private bool CheckIsInputLegal(bool isConfirm, out string msg, out int inputWeight) {
        // NOTE:
        // DO NOT show warning message if input is empty when entering content,
        // BUT show warning message if input is empty when pressed confirm button

        msg = string.Empty;
        inputWeight = 0;
        string inputNameStr = _inputName.text;
        if (inputNameStr.Length > 10) {
            _inputName.SetTextWithoutNotify(inputNameStr.Substring(0, 10));
        }

        string inputWeightStr = _inputWeight.text;
        if (inputWeightStr.Length > 10) {
            _inputWeight.SetTextWithoutNotify(inputWeightStr.Substring(0, 10));
        }

        if (isConfirm) {
            if (string.IsNullOrEmpty(inputNameStr)) {
                msg = string.Format("You have to enter name.");
                return false;
            }
            else if (string.IsNullOrEmpty(inputWeightStr)) {
                msg = string.Format("You have to enter weight.");
                return false;
            }
        }

        bool isModify = RestaurantHandler.HasRestaurant(_targetIndex);
        if (isModify) {
            string oriName = RestaurantHandler.GetRestaurant(_targetIndex).Name;
            if (oriName == inputNameStr) {
                // Do nothing
            }
            else {
                if (RestaurantHandler.HasRestaurant(inputNameStr)) {
                    msg = string.Format("Duplicated name.");
                    return false;
                }
            }
        }
        else {
            if (string.IsNullOrEmpty(inputNameStr)) {
                // No warning message 
                return false;
            }
            else if (RestaurantHandler.HasRestaurant(inputNameStr)) {
                msg = string.Format("Duplicated name.");
                return false;
            }
        }

        if (!int.TryParse(inputWeightStr, out inputWeight)) {
            msg = string.Format("Please enter number.");
            return false;
        }

        return true;
    }

    #region UI Input Field Handlings
    private void InputNameChanged(string s) {
        bool isLegal = CheckIsInputLegal(false, out string msg, out int inputWeight);

        ShowWarningMessage(msg);
        _btnConfirm.interactable = isLegal;
    }

    private void InputWeightChanged(string s) {
        bool isLegal = CheckIsInputLegal(false, out string msg, out int inputWeight);

        ShowWarningMessage(msg);
        _btnConfirm.interactable = isLegal;
    }
    #endregion

    #region UI Button Handlings
    private void ButtonConfirmOnClick() {
        bool isLegal = CheckIsInputLegal(true, out string msg, out int inputWeight);
        if (!isLegal) {
            ShowWarningMessage(msg);
            return;
        }

        string inputNameStr = _inputName.text;
        bool isModify = RestaurantHandler.HasRestaurant(_targetIndex);
        RestaurantData newRData = new RestaurantData { Name = inputNameStr, Weight = inputWeight };
        if (isModify) {
            RestaurantHandler.ModifyRestaurant(_targetIndex, newRData);
        }
        else {
            RestaurantHandler.AddNewRestaurant(newRData);
        }

        _goRoot.SetActive(false);
    }

    private void ButtonCancleOnClick() {
        _goRoot.SetActive(false);
    }
    #endregion

    #region Restaurant Data Handlings
    private void RestaurantDataChanged(int index, string defaultName, int defaultWeight) {
        ShowInputMenu(index, defaultName, defaultWeight);
    }
    #endregion

    #region Internal Methods
    private void InitUI() {
        _goRoot.SetActive(false);

        _btnConfirm.interactable = false;
        _textWarningMsg.text = string.Empty;
    }

    private void ShowInputMenu(int index, string defaultName, int defaultWeight) {
        _goRoot.SetActive(true);

        _targetIndex = index;
        _inputName.SetTextWithoutNotify(defaultName);
        _inputWeight.SetTextWithoutNotify(defaultWeight.ToString());
        _btnConfirm.interactable = false;
    }

    private void ShowWarningMessage(string s) {
        _textWarningMsg.text = s;
    }
    #endregion
}
