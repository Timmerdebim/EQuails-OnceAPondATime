using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CornerHeadController : MonoBehaviour
{
    private bool _state = false;
    private bool _black = true;
    private bool _revealed = false; //if npc is not mentioned yet in story do not show indicator (i.e., start only Mosswick is revealed)

    [SerializeField] private Image _image;
    [SerializeField] private CanvasGroup _canvasGroup;
    private Tween _tween;

    void Awake()
    {
        _canvasGroup.alpha = 0;
        transform.SetAsFirstSibling();
    }

    public void SetState(bool state, bool instant = false)
    {
        if (_state == state) return;
        _state = state;
        UpdateVisuals(instant);
    }
    public void MakeNotBlack() => _black = false;

    public void RevealNpc()
    {
        _revealed = true;
        UpdateVisuals();
    }


    private void UpdateVisuals(bool instant = false)
    {
        _tween?.Kill();
        if (_state && _revealed)
        {
            if (_black) _image.color = Color.black;
            else _image.color = Color.white;
            if (instant)
            {
                _canvasGroup.alpha = 1;
                transform.SetAsFirstSibling();
            }
            else
            {
                _tween = _canvasGroup.DOFade(1, 0.2f).SetEase(Ease.InOutQuad)
                .OnComplete(() => transform.SetAsFirstSibling());
            }
        }
        else
        {
            if (instant)
            {
                _canvasGroup.alpha = 0;
                transform.SetAsLastSibling();
            }
            else
            {
                _tween = _canvasGroup.DOFade(0, 0.2f).SetEase(Ease.InOutQuad)
                .OnComplete(() => transform.SetAsLastSibling());
            }
        }
    }
}
