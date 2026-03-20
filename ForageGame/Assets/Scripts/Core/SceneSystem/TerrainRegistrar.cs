using System;
using UnityEngine;

/// <summary>
/// Sorry Tim but I need to detect when the 'active' Terrain changes for the footstep audio
/// This just emits an event when the terrain is done loading so I can account for it
/// Put it on the terrains of each scene and everything is okii
/// 
/// ~Lars
/// </summary>

public class TerrainRegistrar : MonoBehaviour
{
    public static event Action<Terrain> OnTerrainLoaded;
    public static event Action<Terrain> OnTerrainUnloaded;

    private void OnEnable() => OnTerrainLoaded?.Invoke(GetComponent<Terrain>());
    private void OnDisable() => OnTerrainUnloaded?.Invoke(GetComponent<Terrain>());
}
