using TMPro;
using UnityEngine;

public class PopupTextbox : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private TMP_Text _textbox;
    [SerializeField] private TMProOutline _textOutline;

    void Start()
    {
        SetTextColor(_textbox.color);
    }

    public void SetText(string text)
    {
        _textbox.text = text;
    }

    public void SetTextColor(Color color)
    {
        _textbox.color = color;
        Color.RGBToHSV(color, out float hue, out float saturation, out float value); //HSV are all 0 to 1
        hue = (float)((hue + 0.5) % 1); // complementary hue to improve contrast
        value = (float)((value + 0.5) % 1); // complementary value to improve visability
        saturation /= 2; // desaturate to not stand out over the main text
        _textOutline.SetColor(Color.HSVToRGB(hue, saturation, value));
    }

    public void ShowTextbox(bool showTextbox)
    {
        _animator.SetBool("Show", showTextbox);
    }
}
