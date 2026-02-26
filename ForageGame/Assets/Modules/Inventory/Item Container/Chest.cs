using System.Collections.Generic;
using UnityEngine;

public class Chest : ItemContainer
{


    #region Save & Load

    public void SetData(List<InventorySlotData> data)
    {
        for (int i = 0; i < data.Count; i++)
            Slots[i].SetData(data[i]);
    }

    public List<InventorySlotData> GetData()
    {
        List<InventorySlotData> hotbarData = new();

        foreach (var slot in Slots)
            hotbarData.Add(slot.GetData());

        return hotbarData;
    }

    #endregion
}
