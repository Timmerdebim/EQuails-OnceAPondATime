// using System;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine.SceneManagement;
// using System.Collections;

// namespace Project.SceneLoading
// {
//     public class SceneLoader : MonoBehaviour
//     {
//         // [SerializeField] SceneData[] allScenes;
//         private bool isBusy;

//         #region Scene Loading

//         public void LoadScene(SceneInfo scene, Action callback = null) =>
//             LoadScenes(new List<SceneInfo> { scene }, callback);

//         public void LoadScenes(List<SceneInfo> scenes, Action callback = null)
//         {
//             if (isBusy)
//                 queue.Enqueue(new SceneQueueItem(scenes, callback, true)); // Add to the queue
//             else
//             {
//                 isBusy = true;
//                 StartCoroutine(LoadScenesAsync(scenes, callback));
//             }
//         }

//         private IEnumerator LoadScenesAsync(List<SceneInfo> scenes, Action callback = null)
//         {
//             // Debug.Log($"Scene Loader: Loading scenes {scenes}.");

//             foreach (SceneInfo scene in scenes)
//             {
//                 while (scene.state == SceneState.Unloading)
//                     yield return null;
//                 if (scene.state == SceneState.Unloaded)
//                     scene.Load(false);
//             }

//             while (!scenes.All(scene => scene.state == SceneState.AwaitingActivation))
//                 yield return null;

//             foreach (SceneInfo scene in scenes)
//             {
//                 scene.Activate();
//                 scene.ResetOperation();
//             }

//             isBusy = false;
//             // Debug.Log($"Scene Loader: Loaded scenes {scenes}.");
//             UpdateQueue();
//             callback?.Invoke();
//         }

//         #endregion

//         #region Scene Unloading

//         public void UnloadScene(SceneInfo scene, Action callback = null) =>
//              UnloadScenes(new List<SceneInfo> { scene }, callback);

//         public void UnloadScenes(List<SceneInfo> scenes, Action callback = null)
//         {
//             if (isBusy)
//                 queue.Enqueue(new SceneQueueItem(scenes, callback, false)); // Add to the queue
//             else
//             {
//                 isBusy = true;
//                 StartCoroutine(UnloadScenesAsync(scenes, callback));
//             }
//         }

//         private IEnumerator UnloadScenesAsync(List<SceneInfo> scenes, Action callback = null)
//         {
//             // Debug.Log($"Scene Loader: Unloading scenes {scenes}.");

//             foreach (SceneInfo scene in scenes)
//             {
//                 while (scene.state == SceneState.Loading)
//                     yield return null;
//                 if (scene.state == SceneState.Loaded)
//                     scene.Unload();
//             }

//             while (!scenes.All(scene => scene.state == SceneState.Unloaded))
//                 yield return null;

//             foreach (SceneInfo scene in scenes)
//                 scene.ResetOperation();

//             isBusy = false;
//             // Debug.Log($"Scene Loader: Unloaded scenes {scenes}.");
//             UpdateQueue();
//             callback?.Invoke();
//         }

//         #endregion

//         #region Scene Queue

//         private Queue<SceneQueueItem> queue = new();
//         public struct SceneQueueItem
//         {
//             public SceneQueueItem(List<SceneInfo> scenes, Action callback, bool toLoad)
//             {
//                 Scenes = scenes;
//                 Callback = callback;
//                 ToLoad = toLoad;
//             }
//             public List<SceneInfo> Scenes { get; }
//             public Action Callback { get; }
//             public bool ToLoad { get; }
//         }

//         public void UpdateQueue()
//         {
//             if (queue.Count > 0)
//             {
//                 SceneQueueItem queueItem = queue.Dequeue();
//                 if (queueItem.ToLoad)
//                     LoadScenes(queueItem.Scenes, queueItem.Callback);
//                 else
//                     UnloadScenes(queueItem.Scenes, queueItem.Callback);
//             }
//         }

//         #endregion

//         #region Scene Groups

//         [SerializeField] private List<SceneInfo> allScenes = new();

//         public void FullLoadSceneGroup(SceneGroup sceneGroup, Action callback = null) =>
//             UnloadScenes(GetScenesNotGroup(sceneGroup), () => { LoadScenesByGroup(sceneGroup, callback); });

//         public void LoadScenesByGroup(SceneGroup sceneGroup, Action callback = null) =>
//             LoadScenes(sceneGroup.GetScenes(), callback);

//         public void UnloadScenesByGroup(SceneGroup sceneGroup, Action callback = null) =>
//             UnloadScenes(sceneGroup.GetScenes(), callback);

//         public List<SceneInfo> GetScenesNotGroup(SceneGroup sceneGroup)
//         {
//             List<SceneInfo> scenes = new();
//             foreach (SceneInfo scene in allScenes)
//             {
//                 if (!sceneGroup.GetScenes().Contains(scene))
//                     scenes.Add(scene);
//             }
//             return scenes;
//         }
//         #endregion
//     }
// }