using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TMPro;

namespace Project.Menus.FileSelect
{
    public class SaveSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button slotButton;
        [SerializeField] private TMP_Text slotText;
        [SerializeField] private TMP_Text playtimeText;
        [SerializeField] private Button deleteButton;

        [Header("Settings")]
        [SerializeField] private int slotIndex;


        public void Refresh()
        {
            if (SaveSystem.SaveFileExists(slotIndex))
            {
                SaveData data = SaveSystem.GetSaveFile(slotIndex);
                slotText.text = "continue"; // TODO: add duck progress image?
                playtimeText.text = FormatPlaytime(data.playtimeSeconds);
                deleteButton.gameObject.SetActive(true);
            }
            else
            {
                slotText.text = "New Game";
                playtimeText.text = "";
                deleteButton.gameObject.SetActive(false);
            }
        }

        // ------------ Buttons ------------

        public void OnSlotSelected()
        {
            if (SaveSystem.SaveFileExists(slotIndex))
                GameManager.Instance.PlayGame(slotIndex);   // Load game with this save file
            else
                GameManager.Instance.PlayNewGame(slotIndex);    // Create new game in this slot
        }

        public void OnDeleteSlot()
        {
            if (SaveSystem.SaveFileExists(slotIndex))
            {
                SaveSystem.DeleteSaveFile(slotIndex);
                Refresh();
            }
        }

        // ------------ Functions ------------

        private string FormatPlaytime(float seconds)
        {
            int hours = (int)(seconds / 3600);
            int minutes = (int)((seconds % 3600) / 60);
            return $"{hours}h {minutes}m";
        }
    }
}