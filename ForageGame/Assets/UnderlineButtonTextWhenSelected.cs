using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UnderlineButtonTextWhenSelected : MonoBehaviour,
    ISelectHandler,
    IDeselectHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    Button button;
    TextMeshProUGUI buttonText;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Highlight(bool highlight)
    {
        if (highlight)
        {
            buttonText.fontStyle |= FontStyles.Underline;
        }
        else
        {
            buttonText.fontStyle &= ~FontStyles.Underline;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        Highlight(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Highlight(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlight(false);
    }
}
