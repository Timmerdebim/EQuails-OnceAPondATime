using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameHintElement : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private InputActionReference _inputActionReference;
    [SerializeField] private Image _image;
    private Tween tween;
    private bool _isShown = false;

    void Start() => Hide(true);

    public void SetActive(bool show, bool instant = false)
    {
        if (show) Show(instant);
        else Hide(instant);
    }

    public void Show(bool instant = false)
    {
        if (_isShown) return;
        else _isShown = true;

        gameObject.SetActive(true);
        RefreshVisuals();

        tween?.Kill();
        if (instant)
            _canvasGroup.alpha = 1;
        else
            tween = _canvasGroup.DOFade(1, 1).SetEase(Ease.InOutQuad);
    }

    public void Hide(bool instant = false)
    {
        if (!_isShown) return;
        else _isShown = false;

        tween?.Kill();
        if (instant)
        {
            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
        else
        {
            tween = _canvasGroup.DOFade(0, 0.2f).SetEase(Ease.InOutQuad)
            .OnComplete(() => gameObject.SetActive(false));
        }
    }

    public void RefreshVisuals()
    {
        _image.sprite = KeybindSystem.GetKeybindVisual(_inputActionReference.action, KeybindSystem.DeviceType.LastUsed);
    }
}
