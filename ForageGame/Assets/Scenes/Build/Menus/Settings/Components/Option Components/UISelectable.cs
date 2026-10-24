using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UISelectable : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [SerializeField] private EventReference selectSfx;
    private Selectable selectable;

    void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

    //hover over selects this gameObject with the cursor too
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectable.interactable) EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        RuntimeManager.PlayOneShot(selectSfx);
    }
}
