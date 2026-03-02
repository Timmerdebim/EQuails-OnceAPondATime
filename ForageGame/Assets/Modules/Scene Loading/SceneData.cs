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
    [HideInInspector] public SceneState state => GetSceneState();
    private AsyncOperation operation;
    private bool lastFunctionWasLoad = false; // true = loading, false = unloading

    public void Load(bool allowSceneActivation)
    {
        lastFunctionWasLoad = true;
        operation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        operation.allowSceneActivation = allowSceneActivation;
    }

    public void Activate()
    {
        if (state == SceneState.AwaitingActivation)
            operation.allowSceneActivation = true;
    }

    public void Unload()
    {
        lastFunctionWasLoad = false;
        operation = SceneManager.UnloadSceneAsync(name);
    }

    public SceneState GetSceneState()
    {
        if (IsLoading())
            return SceneState.Loading;
        else if (IsAwaitingActivation())
            return SceneState.AwaitingActivation;
        else if (IsLoaded())
            return SceneState.Loaded;
        else if (IsUnloading())
            return SceneState.Unloading;
        else // if (IsUnloaded())
            return SceneState.Unloaded;
    }

    // ------------ Helper Functions ------------

    public void ResetOperation()
    {
        operation = null;
    }

    private bool IsLoading()
    {
        if (operation == null)
            return false;
        if (operation.progress >= 0.9f) // not isDone!!! (Otherwise await activation won't work!)
            return false;
        if (!lastFunctionWasLoad)
            return false;
        return true;
    }

    private bool IsAwaitingActivation()
    {
        if (operation == null)
            return false;
        if (operation.progress < 0.9f)
            return false;
        if (!lastFunctionWasLoad)
            return false;
        return true;
    }

    private bool IsLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == name && scene.isLoaded)
                return true;
        }
        return false;
    }

    private bool IsUnloading()
    {
        if (operation == null)
            return false;
        if (operation.isDone)
            return false;
        if (lastFunctionWasLoad)
            return false;
        return true;
    }

    private bool IsUnloaded()
    {
        return !IsLoading() && !IsAwaitingActivation() && !IsLoaded() && !IsUnloading();
    }

    // Gets the loading progress of a scene (0-1)
    // 0 to 0.9 are loading, 1 is awaiting activation
    private float GetOperationProgress()
    {
        if (operation != null)
            return operation.progress;
        return 0f;
    }
}

public enum SceneGroup
{
    MainMenu, World, Debug, Pause,
    Swamp, Cave, Mushroom, Flower, Home
}

public enum SceneState
{
    Unloading, Unloaded, Loading, AwaitingActivation, Loaded
}