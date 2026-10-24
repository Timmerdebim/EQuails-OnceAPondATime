using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;
using UnityEngine.EventSystems;

public class UISelectable : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [SerializeField] private EventReference selectSfx;

    //hover over selects this gameObject with the cursor too
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        //RuntimeManager.PlayOneShot(selectSfx);
    }
}
