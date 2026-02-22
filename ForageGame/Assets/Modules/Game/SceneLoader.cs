using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] SceneData[] allScenes;
    private bool isBusy;

    // ------------ Load Scenes ------------

    public void LoadScenes(List<SceneData> scenes, Action callback = null)
    {
        if (isBusy)
            queue.Enqueue(new SceneQueueItem(scenes, callback, true)); // Add to the queue
        else
        {
            isBusy = true;
            StartCoroutine(LoadScenesAsync(scenes, callback));
        }
    }

    private IEnumerator LoadScenesAsync(List<SceneData> scenes, Action callback = null)
    {
        Debug.Log($"SCENE: Loading scenes.");

        foreach (SceneData scene in scenes)
        {
            while (scene.IsUnloading())
                yield return null;
            if (scene.IsUnloaded())
                scene.Load();
        }

        while (!scenes.All(scene => scene.IsLoaded()))
            yield return null;

        foreach (SceneData scene in scenes)
            scene.ResetOperation();

        isBusy = false;
        Debug.Log($"SCENE: Scenes loaded.");
        callback?.Invoke();
    }

    // ------------ Unload Scenes ------------

    public void UnloadScenes(List<SceneData> scenes, Action callback = null)
    {
        if (isBusy)
            queue.Enqueue(new SceneQueueItem(scenes, callback, false)); // Add to the queue
        else
        {
            isBusy = true;
            StartCoroutine(UnloadScenesAsync(scenes, callback));
        }
    }

    private IEnumerator UnloadScenesAsync(List<SceneData> scenes, Action callback = null)
    {
        Debug.Log($"SCENE: Unloading scenes.");

        Debug.Log(5);

        foreach (SceneData scene in scenes)
        {
            while (scene.IsLoading())
                yield return null;

            if (scene.IsLoaded())
                scene.Unload();
        }

        while (!scenes.All(scene => scene.IsUnloaded()))
            yield return null;

        foreach (SceneData scene in scenes)
            scene.ResetOperation();

        isBusy = false;
        Debug.Log($"SCENE: Scenes unloaded.");
        callback?.Invoke();
    }

    // ------------ Queue ------------

    private Queue<SceneQueueItem> queue = new Queue<SceneQueueItem>();
    public struct SceneQueueItem
    {
        public SceneQueueItem(List<SceneData> scenes, Action callback, bool toLoad)
        {
            Scenes = scenes;
            Callback = callback;
            ToLoad = toLoad;
        }
        public List<SceneData> Scenes { get; }
        public Action Callback { get; }
        public bool ToLoad { get; }
    }

    public void UpdateQueue()
    {
        if (queue.Count > 0)
        {
            SceneQueueItem queueItem = queue.Dequeue();
            if (queueItem.ToLoad)
                LoadScenes(queueItem.Scenes, queueItem.Callback);
            else
                UnloadScenes(queueItem.Scenes, queueItem.Callback);
        }
    }

    // ------------ Scene Groups ------------
    public void FullLoadSceneGroup(SceneGroup sceneGroup, Action callback = null)
    {
        UnloadScenes(GetScenesByNotGroup(sceneGroup), () => { LoadScenesByGroup(sceneGroup, callback); });
    }

    public void LoadScenesByGroup(SceneGroup sceneGroup, Action callback = null)
    {
        LoadScenes(GetScenesByGroup(sceneGroup), callback);
    }

    public void UnloadScenesByGroup(SceneGroup sceneGroup, Action callback = null)
    {
        UnloadScenes(GetScenesByGroup(sceneGroup), callback);
    }

    public List<SceneData> GetScenesByGroup(SceneGroup sceneGroup)
    {
        List<SceneData> scenes = new List<SceneData>();
        foreach (SceneData scene in allScenes)
        {
            if (scene.groups.Contains(sceneGroup))
                scenes.Add(scene);
        }
        return scenes;
    }

    public List<SceneData> GetScenesByNotGroup(SceneGroup sceneGroup)
    {
        List<SceneData> scenes = new List<SceneData>();
        foreach (SceneData scene in allScenes)
        {
            if (!scene.groups.Contains(sceneGroup))
                scenes.Add(scene);
        }
        return scenes;
    }
}