using UnityEngine;

public class Door_Lever : MonoBehaviour, IHitHandler
{
    [SerializeField] private Door_Gate doorGate;
    private bool leverTriggered = false;
    [SerializeField] private ParticleSystem hitParticles;

    public void Hit(float damage)
    {
        if (leverTriggered)
        {
            // TODO: play "dink" sound effect
        }

        leverTriggered = true;
        // Flip the lever and play particles and play "lever flick" sound effect
        transform.Rotate(new Vector3(0, 180, 0));
        hitParticles.Play();
        doorGate.OpenDoor();
    }
}
