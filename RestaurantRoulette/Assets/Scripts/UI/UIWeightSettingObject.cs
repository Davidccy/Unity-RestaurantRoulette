using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWeightSettingObject : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Button _btnModify = null;
    [SerializeField] private TextMeshProUGUI _textName = null;
    [SerializeField] private TextMeshProUGUI _textWeight = null;
    [SerializeField] private TextMeshProUGUI _textWeightPercentage = null;
    [SerializeField] private Image _image = null;
    #endregion

    #region Internal Fields
    private int _index;
    private Action<int> _actionModify = null;
    #endregion

    #region Properties
    public int Index {
        get {
            return _index;
        }
    }
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btnModify.onClick.AddListener(ButtonModifyOnClick);
    }

    private void OnDestroy() {
        _btnModify.onClick.RemoveAllListeners();
    }
    #endregion

    #region UI Button Handlings
    private void ButtonModifyOnClick() {
        if (_actionModify == null) {
            return;
        }

        _actionModify(_index);
    }
    #endregion

    #region APIs
    public void SetModifyCallback(Action<int> cb) {
        _actionModify = cb;
    }

    public void SetIndex(int index) {
        _index = index;
    }

    public void SetName(string name) {
        _textName.text = name;
    }

    public void SetWeight(int weight, int totalWeight) {
        _textWeight.text = string.Format("W = {0}", weight);

        float percentage = ((float) weight / totalWeight) * 100;
        _textWeightPercentage.text = string.Format("P = {0:0.000}%", percentage);
    }

    public void SetColor(Color color) {
        _image.color = color;
    }
    #endregion
}
