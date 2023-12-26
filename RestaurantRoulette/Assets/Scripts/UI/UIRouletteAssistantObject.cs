using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRouletteAssistantObject : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _textName = null;
    [SerializeField] private TextMeshProUGUI _textWeight = null;
    [SerializeField] private TextMeshProUGUI _textWeightPercentage = null;
    [SerializeField] private Image _image = null;

    [SerializeField] private RectTransform _rectDirectiveLineStart = null;
    [SerializeField] private RectTransform _rectDirectiveLine = null;
    #endregion

    #region Internal Fields
    private RectTransform _rectDirectiveLineEnd;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        RestaurantHandler.RegisterStatusChangedCallback(RouletteStatusChanged);
    }

    private void OnDestroy() {
        RestaurantHandler.UnregisterStatusChangedCallback(RouletteStatusChanged);
    }

    private void Update() {
        UpdateDirectiveLine();
    }
    #endregion

    #region Restaurant Handler Related Callbacks
    private void RouletteStatusChanged(RouletteStatus status) {
        _rectDirectiveLine.gameObject.SetActive(status == RouletteStatus.Idle);
    }
    #endregion

    #region APIs
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

    public void SetDirectiveLineEnd(RectTransform rect) {
        _rectDirectiveLineEnd = rect;
    }
    #endregion

    #region Internal Methods
    private void UpdateDirectiveLine() {
        Vector2 diffVector = _rectDirectiveLineEnd.position - _rectDirectiveLineStart.position;
        float distance = Vector2.Distance(_rectDirectiveLineStart.position, _rectDirectiveLineEnd.position);
        float degree = GetVectorDegree(diffVector.normalized);

        _rectDirectiveLine.sizeDelta = new Vector2(distance, _rectDirectiveLine.sizeDelta.y);
        _rectDirectiveLine.localEulerAngles = new Vector3(0, 0, degree);
    }

    private float GetVectorDegree(Vector2 normalizedVector) {
        float tanX = normalizedVector.x != 0 ? normalizedVector.y / normalizedVector.x : 0;
        float x = Mathf.Atan(tanX);
        if (normalizedVector.x == 0) {
            if (normalizedVector.y > 0) {
                x = Mathf.PI / 2;
            }
            else {
                x = -Mathf.PI / 2;
            }
        }
        else if (normalizedVector.x < 0) {
            if (normalizedVector.y > 0) {
                x += Mathf.PI;
            }
            else {
                x -= Mathf.PI;
            }
        }
        float xDegree = x * Mathf.Rad2Deg;

        return xDegree;
    }
    #endregion
}
