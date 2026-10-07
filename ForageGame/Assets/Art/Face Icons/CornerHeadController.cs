using DG.Tweening;
using TDK.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class CornerHeadController : MonoBehaviour, ISaveable, ILoadable
{
    private bool _state = false;
    private bool _black = true;
    private bool _revealed = false; //if npc is not mentioned yet in story do not show indicator (i.e., start only Mosswick is revealed)

    [SerializeField] private Image _image;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Sprite _shownSprite;
    [SerializeField] private Sprite _hiddenSprite;
    [SerializeField] private int _saveInt = 0; // 0, 1, 2, or 3.
    private Tween _tween;

    void Awake()
    {
        UpdateVisuals(true);
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
            if (_black) _image.sprite = _hiddenSprite;
            else _image.sprite = _shownSprite;
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
            if (instant)
                _canvasGroup.alpha = 1;
            else
                _tween = _canvasGroup.DOFade(1, 0.5f).SetEase(Ease.InOutQuad);
        }
        else
        {
            if (instant)
            {
                _canvasGroup.alpha = 0;
                gameObject.SetActive(false);
            }
            else
            {
                _tween = _canvasGroup.DOFade(0, 0.5f).SetEase(Ease.InOutQuad)
                .OnComplete(() => gameObject.SetActive(false));
            }
        }
    }

    public void LoadData(WorldSaveData data)
    {
        _revealed = data.cornerHeadsState[_saveInt];
        _black = data.cornerHeadsBlack[_saveInt];
        UpdateVisuals();
    }

    public void SaveData(ref WorldSaveData data)
    {
        data.cornerHeadsState[_saveInt] = _revealed;
        data.cornerHeadsBlack[_saveInt] = _black;
    }
}
