using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Achievement
{
    public string id;
    public string title;
    public string description;
    public Sprite icon;
    public bool isUnlocked;
    public System.DateTime unlockedTime;
}

public class AchievementsMenu : Menu
{
    [Header("UI References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform achievementContainer;
    [SerializeField] private GameObject achievementItemPrefab;

    [Header("Connected Menus")]
    [SerializeField] private Menu mainMenu;

    private List<Achievement> achievements = new List<Achievement>();

    public override void Escape()
    {
        MenuManager.Instance.ToMenu(mainMenu, true);
    }

    // ------------ Buttons ------------

    // ------------ Functions ------------

    private void PopulateUI()
    {
        // Clear existing items
        foreach (Transform child in achievementContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Achievement achievement in achievements)
        {
            GameObject itemGO = Instantiate(achievementItemPrefab, achievementContainer);

            Image iconImage = itemGO.GetComponentInChildren<Image>();
            Text[] textComponents = itemGO.GetComponentsInChildren<Text>();

            if (achievement.isUnlocked)
            {
                iconImage.sprite = achievement.icon;
                iconImage.color = Color.white;
                textComponents[0].text = achievement.title;
                textComponents[1].text = achievement.description;
                if (textComponents.Length > 2)
                    textComponents[2].text = achievement.unlockedTime.ToString("MMM dd, yyyy");
            }
            else
            {
                iconImage.color = new Color(0.3f, 0.3f, 0.3f);
                textComponents[0].text = "Locked";
                textComponents[1].text = achievement.description;
                if (textComponents.Length > 2)
                    textComponents[2].text = "Locked";
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)achievementContainer); // CHANGED
    }
}