using UnityEngine;

public class FileSlotUI : MonoBehaviour
{
    public int slot;
    public GameObject emptyButton;
    public GameObject continueButton;
    public GameObject deleteButton;

    public void Refresh()
    {
        bool exists = SaveSystem.SaveFileExists(slot);
        emptyButton.SetActive(!exists);
        continueButton.SetActive(exists);
        deleteButton.SetActive(exists);
    }
}

