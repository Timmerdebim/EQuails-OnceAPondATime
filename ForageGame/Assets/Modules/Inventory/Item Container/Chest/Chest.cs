using UnityEngine;

public class Chest : ItemContainer
{


    #region Save & Load

    public void SetData(ItemContainerData data)
    {
        for (int i = 0; i < data.slots.Count; i++)
            Slots[i].SetData(data.slots[i]);
    }

    public ItemContainerData GetData()
    {
        ItemContainerData hotbarData = new();

        foreach (var slot in Slots)
            hotbarData.slots.Add(slot.GetData());

        return hotbarData;
    }

    #endregion
}
