using DG.Tweening;
using UnityEngine;

public class BreakableWall : MonoBehaviour, IHitHandler
{
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private GameObject wallMeshes;
    private bool wallActive = true;

    public void Hit(float damage)
    {
        if (!wallActive)
            return;

        hitParticles.Play();

        // Get the particle duration
        float duration = hitParticles.main.duration;
        // Wait for particle completion
        DOVirtual.DelayedCall(duration / 2, () => DestroyWall());
    }

    private void DestroyWall()
    {
        Collider collider = GetComponent<Collider>();
        wallMeshes.SetActive(false);
        collider.enabled = false;
    }
}
