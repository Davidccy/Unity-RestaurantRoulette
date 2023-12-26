using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRouletteObject : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private RectTransform _rectRoot = null;
    [SerializeField] private RectTransform _rectText = null;
    [SerializeField] private TextMeshProUGUI _textName = null;
    [SerializeField] private Image _image = null;
    [SerializeField] private RectTransform _rectDirectiveLinePointRoot = null;
    [SerializeField] private RectTransform _rectDirectiveLinePoint = null;

    [Header("Performance")]
    [SerializeField] private float _animationTime = 0.8f;
    [SerializeField] private float _animationMaxExtraScale = 0.5f;
    [SerializeField] private AnimationCurve _aniCurve = null;
    #endregion

    #region Coroutines
    private IEnumerator CoAnimation() {
        float passedTime = 0;
        float extraScaleValue = 0;

        while (true) {
            float timeValue = (passedTime % _animationTime) / _animationTime;
            extraScaleValue = _aniCurve.Evaluate(timeValue) * _animationMaxExtraScale;

            _rectRoot.localScale = Vector3.one * (1 + extraScaleValue);

            yield return new WaitForEndOfFrame();

            passedTime += Time.deltaTime;
        }
    }
    #endregion

    #region Properties
    public RectTransform RectDirectiveLinePoint {
        get {
            return _rectDirectiveLinePoint;
        }
    }
    #endregion

    #region APIs
    public void SetName(string name) {
        _textName.text = name;
    }

    public void SetColor(Color color) {
        _image.color = color;
    }

    public void SetWeight(int weight, int totalWeight) {
        if (totalWeight == 0) {
            Debug.LogErrorFormat("Unexpected total weight 0");
        }

        float fillAmount = totalWeight != 0 ? (float) weight / totalWeight : 0;
        _image.fillAmount = fillAmount;

        float angle = weight == totalWeight ? (90 - 180 * 0.5f) : (90 - 180 * fillAmount);
        _rectText.localRotation = Quaternion.Euler(0, 0, angle);
        _rectDirectiveLinePointRoot.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetAngle(float angle) {
        _rectRoot.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void PlayAnimation() {
        StartCoroutine(CoAnimation());
    }

    public void StopAnimation() {
        StopAllCoroutines();

        _rectRoot.localScale = Vector3.one;
    }
    #endregion
}
