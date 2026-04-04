// using UnityEngine;
// using System.IO;
// using DG.Tweening;
// using UnityEngine.UI;
// using System;
// using Project.Menus;
// using Project.SceneLoading;
// using System.ComponentModel;
// using TDK.SaveSystem;
// using UnityEngine.SceneManagement;

// public class AppServices : MonoBehaviour
// {
//     [SerializeField] private bool _bootMode = false;

//     public enum State { MainMenu, Gameplay, Cutscene, Transitioning };
//     public State _state { get; private set; } = State.Transitioning;

//     public static AppServices Instance;
//     public SceneLoader sceneLoader { get; private set; }

//     [Header("Scene Groups")]
//     [SerializeField] private SceneGroup _mainMenu;
//     [SerializeField] private SceneGroup _gameplay;
//     [SerializeField] private SceneGroup _world;

//     private readonly string[] _worldIds = { "1", "2", "3" };

//     void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;

//         if (_bootMode) ToMainMenu();
//     }

//     public void ToMainMenu()
//     {
//         SetState(State.Transitioning);
//         SaveManager.Instance?.SaveWorld();
//         MenuManager.Instance?.ToMenu(null, false);

//         TransitionServices.Instance.PlayFadeOut(callback: () =>
//         sceneLoader.FullLoadSceneGroup(_mainMenu, () =>
//         TransitionServices.Instance.PlayFadeIn(callback: () =>
//         SetState(State.MainMenu))));
//     }

//     public void ToWorld(string worldId = null)
//     {
//         worldId ??= PlayerPrefs.GetString("lastWorldUsed", null);

//         if (worldId == null || !SaveServices.ExistsWorld(worldId))
//         {
//             ToNewWorld();
//             return;
//         }


//         TransitionServices.Instance.PlayFadeOut(callback: () =>
//             sceneLoader.FullLoadSceneGroup(_gameplay, () =>
//             sceneLoader.FullLoadSceneGroup(_world, () =>
//             SaveManager.Instance.LoadWorld(() =>
//             TransitionServices.Instance.PlayFadeIn(callback: () =>
//             SetState(State.Gameplay))))));
//     }

//     public void ToNewWorld(string worldId = null)
//     {
//         worldId ??= SaveServices.GetFreeWorldId(_worldIds);
//         if (worldId == null || SaveServices.ExistsWorld(worldId))
//         {
//             Debug.LogWarning($"APP: Cannot make a new world at id {worldId}.");
//             return;
//         }

//         TransitionServices.Instance.PlayFadeOut(callback: () =>
//                 sceneLoader.FullLoadSceneGroup(_gameplay, () =>
//                 sceneLoader.FullLoadSceneGroup(_world, () =>
//                 TransitionServices.Instance.PlayFadeIn(callback: () =>
//                 SetState(State.Gameplay)))));
//     }

//     public void Quit()
//     {
//         _state = State.Transitioning;
// #if UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false;
// #else
//         Application.Quit();
// #endif
//     }


//     private void SetState(State state)
//     {
//         _state = state;

//         switch (state)
//         {
//             case State.MainMenu:
//                 Cursor.lockState = CursorLockMode.None;
//                 Cursor.visible = true;
//                 break;
//             case State.Gameplay:
//                 // Cursor.lockState = CursorLockMode.Locked;
//                 Cursor.visible = false;
//                 break;
//             case State.Cutscene:
//                 break;
//             case State.Transitioning:
//                 break;
//         }
//     }
// }
