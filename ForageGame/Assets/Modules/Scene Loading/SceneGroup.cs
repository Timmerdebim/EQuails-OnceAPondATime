using System.Collections.Generic;
using UnityEngine;

namespace Project.SceneLoading
{
    [CreateAssetMenu(fileName = "New Scene Group", menuName = "SceneLoading/SceneGroup")]
    public class SceneGroup : ScriptableObject
    {
        [SerializeField] private List<SceneInfo> scenes;
        public List<SceneInfo> GetScenes() => new(scenes);
    }
}