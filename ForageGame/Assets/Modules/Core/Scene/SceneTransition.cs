// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;
// using UnityEngine.ResourceManagement.ResourceProviders;

// namespace TDK.SceneSystem
// {
//     public class SceneTransitionPlan
//     {
//         private enum Instruction { Load, Unload, Activate }
//         private List<Instruction> _instructions = new();
//         private List<AssetReference> _scenes = new();

//         public SceneTransitionPlan LoadScene(AssetReference scene, bool activateOnLoad = true)
//         {
//             _instructions.Add(Instruction.Load);
//             _scenes.Add(scene);
//             if (activateOnLoad) _instructions.Add(Instruction.Activate);
//             return this;
//         }

//         public SceneTransitionPlan UnloadScene(AssetReference scene)
//         {
//             _instructions.Add(Instruction.Unload);
//             _scenes.Add(scene);
//             return this;
//         }

//         public SceneTransitionPlan ActivateScenes()
//         {
//             _instructions.Add(Instruction.Activate);
//             return this;
//         }

//         public async UniTask Execute()
//         {
//             int sceneIndex = 0;
//             List<AssetReference> _scenesAwaitingActivation = new();
//             foreach (Instruction instruction in _instructions)
//             {
//                 switch (instruction)
//                 {
//                     case Instruction.Unload:
//                         Addressables.UnloadSceneAsync(_scenes[sceneIndex]);
//                         break;
//                     case Instruction.Load:
//                         AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(_scenes[sceneIndex], UnityEngine.SceneManagement.LoadSceneMode.Additive);
//                         _scenesAwaitingActivation.Add(handle);
//                         break;
//                     case Instruction.Activate:
//                         foreach (AsyncOperationHandle<SceneInstance> handler in _scenesAwaitingActivation)
//                         {
//                             handler.
//                         }
//                         break;
//                 }
//             }
//         }
//     }
// }