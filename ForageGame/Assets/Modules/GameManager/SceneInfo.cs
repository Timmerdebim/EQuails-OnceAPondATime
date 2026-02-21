using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

[Serializable]
public struct SceneInfo
{
    public string name;
    [HideInInspector] public Action loadingCallback;
    [HideInInspector] public AsyncOperation loadingOperation;

    public bool IsSceneLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == name && scene.isLoaded)
                return true;
        }
        return false;
    }

    // Gets the loading progress of a scene (0-1)
    public float GetLoadingProgress()
    {
        if (loadingOperation != null)
            return loadingOperation.progress;
        return 0f;
    }

    public bool IsSceneLoading()
    {
        return (loadingOperation != null);
    }

    public bool IsSceneUnloaded()
    {
        return (!IsSceneLoaded() && !IsSceneLoading());
    }
}