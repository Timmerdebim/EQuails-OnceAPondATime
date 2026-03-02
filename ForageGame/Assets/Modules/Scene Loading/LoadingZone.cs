using UnityEngine;

public class LoadingZone : MonoBehaviour
{
    [SerializeField] private LayerMask triggeringLayers;
    [SerializeField] private SceneGroup loadGroup;

    void OnTriggerEnter(Collider other)
    {
        if ((triggeringLayers.value & (1 << other.gameObject.layer)) != 0)
            GameManager.Instance.sceneLoader.LoadScenesByGroup(loadGroup);
    }

    void OnTriggerExit(Collider other)
    {
        if ((triggeringLayers.value & (1 << other.gameObject.layer)) != 0)
            GameManager.Instance.sceneLoader.UnloadScenesByGroup(loadGroup);
    }
}
