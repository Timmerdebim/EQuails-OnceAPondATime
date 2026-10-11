using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameHintElement : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private InputActionReference _inputActionReference;
    [SerializeField] private Image _image;
    private bool _isShown = false;
    private Tween tween;

    void Awake()
    {
        _canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void SetState(bool showHint)
    {
        if (showHint == _isShown) return;
        _isShown = showHint;

        tween?.Kill();

        if (showHint)
        {
            _image.sprite = KeybindSystem.GetLastUsedKeybindVisual(_inputActionReference.action);
            transform.SetAsFirstSibling();
            gameObject.SetActive(true);
            tween = _canvasGroup.DOFade(1, 1).SetEase(Ease.InOutQuad);
        }
        else
        {
            tween = _canvasGroup.DOFade(0, 0.2f).SetEase(Ease.InOutQuad)
            .OnComplete(() => gameObject.SetActive(false));
        }
    }
}
