using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _particleSystems;
    public void Play()
    {
        Debug.Log("Wha?");
        foreach (ParticleSystem particleSystem in _particleSystems)
            particleSystem.Play();
    }
}
