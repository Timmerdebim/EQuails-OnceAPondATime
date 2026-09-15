using TMPro;
using UnityEngine;

public class PopupTextbox : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private TMP_Text _textbox;

    public void SetText(string text)
    {
        _textbox.text = text;
    }

    public void SetTextColor(Color color)
    {
        _textbox.color = color;
    }

    public void ShowTextbox(bool showTextbox)
    {
        _animator.SetBool("Show", showTextbox);
    }
}
