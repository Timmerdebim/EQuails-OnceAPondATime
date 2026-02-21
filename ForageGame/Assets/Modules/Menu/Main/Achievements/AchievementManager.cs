using UnityEngine;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    private List<Achievement> achievements = new List<Achievement>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        InitializeAchievements();
    }

    private void InitializeAchievements()
    {
        // Example achievements - customize these
        achievements.Add(new Achievement
        {
            id = "first_kill",
            title = "First Blood",
            description = "Defeat your first enemy",
            isUnlocked = false
        });

        achievements.Add(new Achievement
        {
            id = "speedrunner",
            title = "Speedrunner",
            description = "Complete the game in under 1 hour",
            isUnlocked = false
        });

        achievements.Add(new Achievement
        {
            id = "collector",
            title = "Collector",
            description = "Find all hidden items",
            isUnlocked = false
        });
    }

    public void UnlockAchievement(string id)
    {
        Achievement achievement = achievements.Find(a => a.id == id);
        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.isUnlocked = true;
            achievement.unlockedTime = System.DateTime.Now;
            Debug.Log($"Achievement Unlocked: {achievement.title}");
        }
    }

    public List<Achievement> GetAllAchievements() => achievements;
}