using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text slotNumberText;
    [SerializeField] private Image slotImage;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color notSelectedColor;


    private int slotIndex;
    private InventorySystem inventory;
    private Item currentItem;

    public void Initialize(int index, InventorySystem invSystem)
    {
        slotIndex = index;
        inventory = invSystem;

        if (slotNumberText != null)
            slotNumberText.text = (index + 1).ToString();
        if (slotImage != null)
            slotImage.color = notSelectedColor;

        ClearSlot();
    }

    public void SetItem(Item item)
    {
        currentItem = item;
        itemIcon.sprite = item.icon;
        itemIcon.enabled = true;
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
            slotImage.color = selectedColor;
        else
            slotImage.color = notSelectedColor;
    }



    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     if (currentItem != null && eventData.button == PointerEventData.InputButton.Right)
    //     {
    //         // Drop item at player's position
    //         Vector3 dropPosition = Camera.main.transform.position + Camera.main.transform.forward * 2f;
    //         inventory.DropItem(slotIndex, dropPosition);
    //     }
    // }
}