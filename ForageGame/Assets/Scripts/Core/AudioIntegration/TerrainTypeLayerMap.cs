using UnityEngine;
using System.Collections.Generic;


public enum TerrainType { Grass, Dirt, Sand, Rock, Snow, Wood }

[System.Serializable]
public struct TerrainMapEntry
{
    public TerrainLayer layer;
    public TerrainType type;
}

/// <summary>
/// This is just so we can have a serializable dictionary
/// ~Lars
/// </summary>
[CreateAssetMenu(fileName = "TerrainTypeLayerMap", menuName = "AudioStuff/TerrainTypeLayerMap")]
public class TerrainTypeLayerMap : ScriptableObject
{
    public TerrainMapEntry[] entries;
}
