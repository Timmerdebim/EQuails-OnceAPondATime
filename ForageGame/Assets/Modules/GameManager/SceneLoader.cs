using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneLoader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool debugMode = false;

    [Header("Scenes")]
    [SerializeField] private SceneInfo[] mainMenuScenes;
    [SerializeField] private SceneInfo[] debugScenes;
    [SerializeField] private SceneInfo[] gameScenes;
    [SerializeField] private SceneInfo mainMenuScene;
    [SerializeField] private SceneInfo gameScene;
    [SerializeField] private SceneInfo debugScene;
    [SerializeField] private SceneInfo pauseMenuScene;
    [SerializeField] private SceneInfo islandScene;
    [SerializeField] private SceneInfo forestScene;
    [SerializeField] private SceneInfo swampScene;
    [SerializeField] private SceneInfo flowerScene;
    [SerializeField] private SceneInfo caveScene;
    [SerializeField] private SceneInfo emptyScene;
    [SerializeField] private SceneInfo boarderScene;

    private bool isSingleSceneLoading = false;
    private int additiveLoadersActive = 0;

    public void ToMainMenu(Action callback = null)
    {
        LoadScene(mainMenuScene, callback);
    }

    public void ToGameScene(Action callback = null)
    {
        LoadScene(gameScene, callback);
        if (debugMode)
            LoadScene(debugScene);
        LoadScene(emptyScene, callback);
    }

    // TODO: 
    // private void UpdateLoadedScenes()
    // {
    //     Vector3 playerPos = Player.Instance.transform.position;

    //     if (Vector3.Distance(playerPos, islandScene.position) < islandScene.radius)
    //     {
    //         if (islandScene.scene.isLoaded) return;
    //         else SceneManager.LoadSceneAsync(islandScene.scene.buildIndex, LoadSceneMode.Additive);
    //     }
    //     else
    //     {
    //         if (!islandScene.scene.isLoaded) return;
    //         else SceneManager.UnloadSceneAsync(islandScene.scene.buildIndex);
    //     }
    // }

    // ------------ Load Scene ------------

    public void LoadScene(SceneInfo sceneInfo, Action callback = null)
    {
        if (sceneInfo.IsSceneLoaded())
        {
            Debug.LogWarning($"SCENE: ERROR: Scene '{sceneInfo.name}' is already loaded!");
            callback?.Invoke();
            return;
        }

        if (sceneInfo.IsSceneLoading())
        {
            Debug.LogWarning($"SCENE: ERROR: Scene '{sceneInfo.name}' is already being loaded!");
            // Add to existing callback chain
            if (callback == null)
                sceneInfo.loadingCallback = callback;
            else
                sceneInfo.loadingCallback += callback;
            return;
        }

        sceneInfo.loadingCallback = callback;

        StartCoroutine(LoadSceneAsync(sceneInfo));
    }

    private IEnumerator LoadSceneAsync(SceneInfo sceneInfo)
    {
        Debug.Log($"SCENE: Loading scene '{sceneInfo.name}'.");

        additiveLoadersActive += 1;
        // Wait for single loader to complete
        while (isSingleSceneLoading == true)
            yield return null;

        sceneInfo.loadingOperation = SceneManager.LoadSceneAsync(sceneInfo.name, LoadSceneMode.Additive);

        while (!sceneInfo.loadingOperation.isDone)
            yield return null;

        sceneInfo.loadingCallback?.Invoke();

        sceneInfo.loadingOperation = null;
        sceneInfo.loadingCallback = null;
        additiveLoadersActive -= 1;

        Debug.Log($"SCENE: Loaded scene '{sceneInfo.name}'.");
    }

    // ------------ Unload Additive ------------

    public void UnloadScene(SceneInfo sceneInfo, Action callback = null)
    {
        if (sceneInfo.IsSceneLoaded())
        {
            Debug.LogWarning($"SCENE: ERROR: Scene '{sceneInfo.name}' is already loaded!");
            callback?.Invoke();
            return;
        }

        if (sceneInfo.IsSceneLoading())
        {
            Debug.LogWarning($"SCENE: ERROR: Scene '{sceneInfo.name}' is already being loaded!");
            // Add to existing callback chain
            if (callback == null)
                sceneInfo.loadingCallback = callback;
            else
                sceneInfo.loadingCallback += callback;
            return;
        }

        sceneInfo.loadingCallback = callback;
        StartCoroutine(UnloadSceneAsync(sceneInfo));
    }

    private IEnumerator UnloadSceneAsync(SceneInfo sceneInfo)
    {
        additiveLoadersActive += 1;

        // Wait for single loader to complete
        while (isSingleSceneLoading == true)
            yield return null;

        sceneInfo.loadingOperation = SceneManager.UnloadSceneAsync(sceneInfo.name);

        while (!sceneInfo.loadingOperation.isDone)
            yield return null;

        sceneInfo.loadingCallback?.Invoke();

        sceneInfo.loadingOperation = null;
        sceneInfo.loadingCallback = null;
        additiveLoadersActive -= 1;

        Debug.Log($"Scene '{sceneInfo.name}' unloaded successfully!");
    }
}