using UnityEngine;
using UnityEditor;

public class RescaleTerrainHeightmap : EditorWindow
{
    [MenuItem("Tools/Fix Terrain Height Scale")]
    static void FixTerrainHeight()
    {
        Terrain terrain = Selection.activeGameObject?.GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("Please select a Terrain object first.");
            return;
        }

        TerrainData data = terrain.terrainData;

        float oldMaxHeight = 600f;
        float newMaxHeight = 50f;
        float scale = oldMaxHeight / newMaxHeight; // = 12

        int w = data.heightmapResolution;
        int h = data.heightmapResolution;

        // Read all heights (normalized 0-1 relative to oldMaxHeight)
        float[,] heights = data.GetHeights(0, 0, w, h);

        // Renormalize so world-space values stay the same
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                heights[y, x] = Mathf.Clamp01(heights[y, x] * scale);

        // Apply new max height FIRST, then write renormalized heights
        data.size = new Vector3(data.size.x, newMaxHeight, data.size.z);
        data.SetHeights(0, 0, heights);

        Debug.Log("Done! Terrain height rescaled without moving geometry.");
    }
}