using TDK.PlayerSystem;
using UnityEngine;
//use when using invisible walls, to prevent idiocy from the player and softlocking themselves
public class PlayerRespawnPositionOverride : MonoBehaviour
{
    [SerializeField] private Vector3 baseRespawnPosition = new(0, 10.5f, 0); //default spawn position, gets overridden by save data or npc scenes (invisible walls)
    [SerializeField] private Vector3 overrideRespawnPosition = new(0, 10.5f, 0); //default spawn position, gets overridden by save data or npc scenes (invisible walls)
    void OnEnable()
    {
        // Debug.Log($"[PlayerRespawnPositionOverride: {transform.parent.gameObject.name}] Overriding player respawn position to {overrideRespawnPosition}");
        Player.Instance.respawnPosition = overrideRespawnPosition;
    }
    void OnDisable()
    {
        // Debug.Log($"[PlayerRespawnPositionOverride: {transform.parent.gameObject.name}] reverting player respawn position");
        Player.Instance.respawnPosition = baseRespawnPosition; //magic numba
    }
}
