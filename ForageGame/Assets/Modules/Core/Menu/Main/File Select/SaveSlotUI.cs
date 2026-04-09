using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TMPro;
using TDK.SaveSystem;

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
        [SerializeField] private string _worldId;


        public void Refresh()
        {
            if (SaveServices.ExistsWorld(_worldId))
            {
                WorldSaveData data = SaveServices.GetWorld(_worldId);
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
            if (SaveServices.ExistsWorld(_worldId))
                _ = AppController.Instance.ToWorld(_worldId);   // Load game with this save file
            else
                _ = AppController.Instance.ToNewWorld(_worldId);    // Create new game in this slot
        }

        public void OnDeleteSlot()
        {
            if (SaveServices.ExistsWorld(_worldId))
            {
                SaveServices.DeleteWorld(_worldId);
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