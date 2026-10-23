using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class SelectionUIElement : MonoBehaviour
{
    [SerializeField] private bool _canLoop = true;
    [SerializeField] private string[] _options;
    [SerializeField] private TMP_Text _textBox;
    [SerializeField] private EventReference onChangeSfx;
    [SerializeField] private EventReference onNoChangeSfx;
    public UnityEvent OnChange;
    public int _currentOption { get; private set; } = 0;

    public void SetOptions(string[] options) => _options = options;

    public void SetCurrentOption(int option, bool playSfx = false)
    {
        if (_options.Length < 1)
        {
            Debug.LogWarning("Options list length is 0: this means you have not assigned any options!");
            return;
        }
        var newOption = _canLoop ? 
            (option % _options.Length + _options.Length) % _options.Length : 
            Math.Clamp(option, 0, _options.Length - 1);
        // if (_canLoop) newOption = (option % _options.Length + _options.Length) % _options.Length;
        // else newOption = Math.Clamp(option, 0, _options.Length - 1);

        if (newOption == _currentOption) //no change, play 'can't change' sfx if applicable
        {
            if(playSfx) RuntimeManager.PlayOneShot(onNoChangeSfx);
        }
        else //only do something when something did change
        {
            _currentOption = newOption;
            if(playSfx) RuntimeManager.PlayOneShot(onChangeSfx);
            RefreshVisuals();
            OnChange.Invoke();
        }
    }

    public void LeftButtonPressed() => SetCurrentOption(_currentOption - 1, true);

    public void RightButtonPressed() => SetCurrentOption(_currentOption + 1, true); 

    public void RefreshVisuals()
    {
        if (0 <= _currentOption && _currentOption < _options.Length)
            _textBox.text = _options[_currentOption];
        else
            _textBox.text = "N/A";
    }
}
