using UnityEngine;

public class TerrainTextureDetector : MonoBehaviour
{
    private Terrain terrain;
    private TerrainData terrainData;
    private Vector3 terrainPosition;


    private void OnEnable()
    {
        TerrainRegistrar.OnTerrainLoaded += HandleTerrainLoaded;
        TerrainRegistrar.OnTerrainUnloaded += HandleTerrainUnloaded;
    }

    private void OnDisable()
    {
        TerrainRegistrar.OnTerrainLoaded -= HandleTerrainLoaded;
        TerrainRegistrar.OnTerrainUnloaded -= HandleTerrainUnloaded;
    }

    private void HandleTerrainLoaded(Terrain t)
    {
        terrain = t;
        terrainData = t.terrainData;
        terrainPosition = t.transform.position;
        Debug.Log($"Terrain {t} loaded!");
    }

    private void HandleTerrainUnloaded(Terrain t)
    {
        if (terrain == t)
        {
            terrain = null;
            terrainData = null;
            Debug.Log($"Terrain {t} unloaded!");
        }
    }

    void Update()
    {
        if(terrain == null) return; //no reason to do anything if no terrain is loaded
        int textureIndex = GetDominantTextureIndex(transform.position);
        if(terrainData.terrainLayers.Length == 0) return; //terrain has no uhhhh terrain (textures)
        string textureName = terrainData.terrainLayers[textureIndex].diffuseTexture.name;
        Debug.Log($"Walking on: {textureName}");
    }

    private int GetDominantTextureIndex(Vector3 worldPos)
    {
        float[] mix = GetTextureMix(worldPos);
        int dominantIndex = 0;
        float maxWeight = 0f;

        for (int i = 0; i < mix.Length; i++)
        {
            if (mix[i] > maxWeight)
            {
                maxWeight = mix[i];
                dominantIndex = i;
            }
        }

        return dominantIndex;
    }

    private float[] GetTextureMix(Vector3 worldPos)
    {
        // Convert world position to terrain-local coordinates (0..1 range)
        float normX = (worldPos.x - terrainPosition.x) / terrainData.size.x;
        float normZ = (worldPos.z - terrainPosition.z) / terrainData.size.z;

        // Convert to alphamap coordinates
        int mapX = Mathf.RoundToInt(normX * (terrainData.alphamapWidth - 1));
        int mapZ = Mathf.RoundToInt(normZ * (terrainData.alphamapHeight - 1));

        // alphamaps[z, x, layerIndex] — note the z/x order!
        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        float[] mix = new float[splatmapData.GetUpperBound(2) + 1];
        for (int i = 0; i < mix.Length; i++)
            mix[i] = splatmapData[0, 0, i];

        return mix;
    }
}
