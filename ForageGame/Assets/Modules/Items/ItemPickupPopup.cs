using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemPickupPopup : MonoBehaviour
{
    public static ItemPickupPopup Instance;

    [Header("UI References")]
    public GameObject panel;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;

    private bool waitingForInput = false;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public IEnumerator ShowPopup(Sprite icon, string name, string description)
    {
        // Pause game
        Time.timeScale = 0f;

        panel.SetActive(true);

        itemIcon.sprite = icon;
        itemName.text = name;
        itemDescription.text = description;

        // Optional small delay so player can't instantly skip
        yield return new WaitForSecondsRealtime(0.3f);

        waitingForInput = true;

        // Wait for any key
        while (!Input.anyKeyDown)
            yield return null;

        waitingForInput = false;

        panel.SetActive(false);

        // Resume game
        Time.timeScale = 1f;
    }
}