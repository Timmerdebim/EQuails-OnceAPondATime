using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneData
{
    public string name;
    public SceneGroup[] groups;
    [HideInInspector] public Scene scene => SceneManager.GetSceneByName(name);
    [HideInInspector] public AsyncOperation operation;
    private bool loadingDirection = false; // true = loading, false = unloading

    public void Load()
    {
        loadingDirection = true;
        operation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    }

    public void Unload()
    {
        loadingDirection = false;
        operation = SceneManager.UnloadSceneAsync(name);
    }

    public bool IsLoading()
    {
        return operation != null && loadingDirection;
    }

    public bool IsLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == name && scene.isLoaded)
                return true;
        }
        return false;
    }

    public bool IsUnloading()
    {
        return operation != null && !loadingDirection;
    }

    public bool IsUnloaded()
    {
        return !IsLoaded() && !IsLoading() && !IsUnloading();
    }

    // Gets the loading progress of a scene (0-1)
    public float GetOperationProgress()
    {
        if (operation != null)
            return operation.progress;
        return 0f;
    }
}

public enum SceneGroup { MainMenu, World, Debug, Pause }