using DG.Tweening;
using UnityEngine;

public class BreakableWall : MonoBehaviour, IHitHandler
{
    [SerializeField] private ParticleSystem hitParticles;
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
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Collider collider = GetComponent<Collider>();
        meshRenderer.enabled = false;
        collider.enabled = false;
    }
}
