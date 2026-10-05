using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CornerHeadController : MonoBehaviour
{
    private bool _state = false;
    private bool _black = true;
    private bool _discovered = false; //has this NPC been seen/mentioned in the story so far. Prevents the popup from even showing

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
        _tween?.Kill();
        if (_state && _discovered)
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
    public void MakeNotBlack() => _black = false;
    public void DiscoverNpc() => _discovered = true;
}
