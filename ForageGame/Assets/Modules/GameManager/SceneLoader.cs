using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct SceneLoaderInfo
{
    public Scene scene;
    public Vector3 position;
    public float radius;
}

public class SceneLoader : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private SceneLoaderInfo debugScene;
    [SerializeField] private SceneLoaderInfo islandScene;
    [SerializeField] private SceneLoaderInfo forestScene;
    [SerializeField] private SceneLoaderInfo swampScene;
    [SerializeField] private SceneLoaderInfo flowerScene;
    [SerializeField] private SceneLoaderInfo caveScene;
    [SerializeField] private SceneLoaderInfo emptyScene;
    [SerializeField] private SceneLoaderInfo boarderScene;

    [Header("Settings")]
    [SerializeField] private bool debugMode = false;

    void Awake()
    {
        if (debugMode)
            SceneManager.LoadSceneAsync(debugScene.scene.buildIndex, LoadSceneMode.Additive);
        else
        {
            SceneManager.LoadSceneAsync(emptyScene.scene.buildIndex, LoadSceneMode.Additive);
        }
    }

    // TODO: 
    private void UpdateLoadedScenes()
    {
        Vector3 playerPos = Player.Instance.transform.position;

        if (Vector3.Distance(playerPos, islandScene.position) < islandScene.radius)
        {
            if (islandScene.scene.isLoaded) return;
            else SceneManager.LoadSceneAsync(islandScene.scene.buildIndex, LoadSceneMode.Additive);
        }
        else
        {
            if (!islandScene.scene.isLoaded) return;
            else SceneManager.UnloadSceneAsync(islandScene.scene.buildIndex);
        }
    }
}
