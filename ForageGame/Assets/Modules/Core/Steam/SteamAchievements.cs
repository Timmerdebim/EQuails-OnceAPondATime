using UnityEngine;
using Steamworks;

/// <summary>
/// Minimal Steam achievements + stats manager (Steamworks.NET).
/// Put this on a GameObject in your first scene. It persists across scenes.
///
/// Usage:
///   SteamAchievements.Instance.Unlock("ACH_FIRST_WIN");
///   SteamAchievements.Instance.AddToStat("STAT_ENEMIES_KILLED", 1, "ACH_KILL_100", 100);
/// </summary>
[DisallowMultipleComponent]
public class SteamAchievements : MonoBehaviour
{
    public static SteamAchievements Instance { get; private set; }

    public bool SteamReady { get; private set; }
    public bool StatsReady { get; private set; }

    Callback<UserStatsReceived_t> _onStatsReceived;
    Callback<UserStatsStored_t> _onStatsStored;
    Callback<UserAchievementStored_t> _onAchievementStored;

    bool _dirty; // stats changed and need storing

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Sanity checks recommended by Steamworks.NET
        if (!Packsize.Test())
            Debug.LogError("[Steam] Packsize test failed. Wrong Steamworks.NET platform build?");
        if (!DllCheck.Test())
            Debug.LogError("[Steam] DllCheck failed. steam_api DLL missing or wrong version.");

        // For release builds you may want:
        // if (SteamAPI.RestartAppIfNecessary(new AppId_t(YOUR_APP_ID))) { Application.Quit(); return; }

        try
        {
            SteamReady = SteamAPI.Init();
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("[Steam] Could not load steam_api: " + e);
            return;
        }

        if (!SteamReady)
        {
            Debug.LogWarning("[Steam] SteamAPI.Init() failed. Is Steam running? Is steam_appid.txt present?");
            return;
        }

        _onStatsReceived = Callback<UserStatsReceived_t>.Create(OnStatsReceived);
        _onStatsStored = Callback<UserStatsStored_t>.Create(OnStatsStored);
        _onAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

        // Newer Steamworks SDKs (1.61+) load the user's stats automatically
        // during SteamAPI.Init(), so RequestCurrentStats() no longer exists.
        StatsReady = true;
    }

    void Update()
    {
        if (!SteamReady) return;
        SteamAPI.RunCallbacks();

        if (_dirty && StatsReady)
        {
            _dirty = false;
            SteamUserStats.StoreStats();
        }
    }

    void OnDestroy()
    {
        if (Instance != this) return;
        if (SteamReady) SteamAPI.Shutdown();
        Instance = null;
    }

    // ---------------------------------------------------------------- Public API

    /// <summary>Unlock an achievement by its API Name (set in Steamworks partner site).</summary>
    public void Unlock(string achievementId)
    {
        if (!CanUse()) return;

        if (SteamUserStats.GetAchievement(achievementId, out bool alreadyUnlocked) && alreadyUnlocked)
            return;

        if (SteamUserStats.SetAchievement(achievementId))
            _dirty = true;
        else
            Debug.LogWarning($"[Steam] SetAchievement failed for '{achievementId}'. Check the API Name.");
    }

    public bool IsUnlocked(string achievementId)
    {
        if (!CanUse()) return false;
        return SteamUserStats.GetAchievement(achievementId, out bool unlocked) && unlocked;
    }

    /// <summary>Set an int stat. Optionally unlock an achievement when the stat reaches a target.</summary>
    public void SetStat(string statId, int value, string unlockId = null, int unlockAt = 0)
    {
        if (!CanUse()) return;
        if (SteamUserStats.SetStat(statId, value))
        {
            _dirty = true;
            if (!string.IsNullOrEmpty(unlockId) && value >= unlockAt)
                Unlock(unlockId);
        }
    }

    /// <summary>Increment an int stat and optionally unlock an achievement at a threshold.</summary>
    public void AddToStat(string statId, int amount, string unlockId = null, int unlockAt = 0)
    {
        if (!CanUse()) return;
        if (!SteamUserStats.GetStat(statId, out int current)) return;
        SetStat(statId, current + amount, unlockId, unlockAt);
    }

    /// <summary>Show the "X / Y" progress popup for a progress-style achievement.</summary>
    public void ShowProgress(string achievementId, uint current, uint max)
    {
        if (!CanUse()) return;
        SteamUserStats.IndicateAchievementProgress(achievementId, current, max);
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    /// <summary>TESTING ONLY: wipes all stats and achievements for the current user.</summary>
    [ContextMenu("Reset All Stats And Achievements")]
    public void ResetAll()
    {
        if (!CanUse()) return;
        SteamUserStats.ResetAllStats(true);
        SteamUserStats.StoreStats();
    }
#endif

    // ---------------------------------------------------------------- Internals

    bool CanUse()
    {
        if (!SteamReady || !StatsReady)
        {
            Debug.LogWarning("[Steam] Not ready yet (Steam not initialized or stats not received).");
            return false;
        }
        return true;
    }

    void OnStatsReceived(UserStatsReceived_t cb)
    {
        if (cb.m_nGameID != SteamUtils.GetAppID().m_AppId) return;

        if (cb.m_eResult == EResult.k_EResultOK)
        {
            StatsReady = true;
            Debug.Log("[Steam] Stats received.");
        }
        else
        {
            Debug.LogError("[Steam] RequestCurrentStats failed: " + cb.m_eResult);
        }
    }

    void OnStatsStored(UserStatsStored_t cb)
    {
        if (cb.m_nGameID != SteamUtils.GetAppID().m_AppId) return;

        if (cb.m_eResult == EResult.k_EResultOK)
            Debug.Log("[Steam] Stats stored.");
        else
            Debug.LogError("[Steam] StoreStats failed: " + cb.m_eResult);
    }

    void OnAchievementStored(UserAchievementStored_t cb)
    {
        if (cb.m_nGameID != SteamUtils.GetAppID().m_AppId) return;
        Debug.Log($"[Steam] Achievement stored: {cb.m_rgchAchievementName} ({cb.m_nCurProgress}/{cb.m_nMaxProgress})");
    }
}