using System;
using UnityEngine;

public class DuckHitHandler : IHitHandler
{
    public DuckController duck;
    public ParticleSystem hitParticles;

    public float iFramesDuration = 0.5f; // Invincibility frames duration in seconds
    private float iFramesTimer = 0f;

    public DuckHitHandler(DuckController duck)
    {
        this.duck = duck;
    }

    public void Awake()
    {
        if (hitParticles)
            hitParticles.Stop();
    }

    public override void Hit(float damage)
    {
        // update invincibility timer
        if (iFramesTimer > 0f) // Debug.Log(gameObject.name + " is invincible and took no damage.");
            return; // currently in invincibility frames
        else
            iFramesTimer = iFramesDuration; // reset invincibility timer
        duck.duckEnergy.TakeDamage(damage);


        // assume hit particles are burst at time 0
        if (hitParticles)
        {
            hitParticles.time = 0f;
            hitParticles.Play();
        }
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + duck.duckEnergy.currentMaxEnergy);
        if (duck.duckEnergy.energy <= 0)
            Debug.Log("bro died");
    }

    // Update is called once per frame
    void Update()
    {
        // update invincibility timer
        if (iFramesTimer > 0f)
        {
            iFramesTimer -= Time.deltaTime;
            if (iFramesTimer < 0f)
                iFramesTimer = 0f;
        }
    }

}