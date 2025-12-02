using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public EnemyController enemy;
    public float visionRadius = 10f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    public bool playerInVisionCone = false;
    public Transform player;
    public bool canSeePlayer = false;
    public bool newCanSeePlayer = false;


    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
            playerInVisionCone = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
            playerInVisionCone = false;
    }

    void Update()
    {
        newCanSeePlayer = false;

        if (playerInVisionCone)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            if (Physics.SphereCast(transform.position, 0.5f, direction, out RaycastHit hit, visionRadius, obstacleMask | playerMask))
            {
                if (hit.transform == player)
                    newCanSeePlayer = true;
            }
        }

        if (canSeePlayer != newCanSeePlayer)
        {
            canSeePlayer = newCanSeePlayer;
            enemy.animator.SetBool("canSeePlayer", canSeePlayer); // Only update if it is new
        }

        if (canSeePlayer)
            enemy.lastSeenPlayerPos = player.position;
    }
}
