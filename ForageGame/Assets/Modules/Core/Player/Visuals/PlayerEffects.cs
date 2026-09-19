using System;
using TDK.PlayerSystem;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    PlayerController pc;

    [SerializeField] private PlayerVisuals pv;
    Rigidbody rb;
    SurfaceTypeDetector surfaceTypeDetector;
    Energy en;

    [SerializeField] private ParticleSystem attackParticles;
    [SerializeField] private ParticleSystem jumpParticles;


    [System.Serializable]
    private struct ParticleOffsets
    {
        public float stepSideXOffset, viewXoffset, motionXoffset, motionZoffset;
    }

    [Header("Particle Offsets")]
    [SerializeField] private ParticleOffsets walkFootstepOffsets;
    [SerializeField] private ParticleOffsets swimFootstepOffsets;
    [SerializeField] private ParticleOffsets waterWakeOffsets;

    [Header("Footstep Particles")]
    [SerializeField] private ParticleSystem waterStepParticles;
    [SerializeField] private ParticleSystem dustParticles;

    [Header("Water / Swimming Particles")]
    [SerializeField] private ParticleSystem swimStrokeParticles;
    [SerializeField] private WakeTrailController wakeTrail;
    [SerializeField] private ParticleSystem waterEnterParticles;
    [SerializeField] private ParticleSystem waterSplashParticles;
    [SerializeField] private float minWaterSplashVelocity;



    [Header("Land Particles")]
    [SerializeField] private ParticleSystem landParticles;

    [SerializeField] private AnimationCurve landParticleSpeedVSParticlecountCurve;
    [SerializeField] private float landParticlesSaturationSpeed;
    [SerializeField] private float landParticlesSaturationCount;

    [Header("Hit Particles")]
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private int hitParticlesSaturationCount = 10;
    private float hitParticlesDamageSaturation;

    private void Awake()
    {
        pc = GetComponentInParent<PlayerController>();
        rb = GetComponentInParent<Rigidbody>();
        surfaceTypeDetector = GetComponentInParent<SurfaceTypeDetector>();
        en = GetComponentInParent<Energy>();
        hitParticlesDamageSaturation = en.currentMaxEnergy;
    }

    private void OnEnable()
    {
        pc.onAttack.AddListener(AttackEffect);
        pc.onJump.AddListener(JumpEffect);
        pc.onLand.AddListener(LandEffect);
        en.onHit.AddListener(HitEffect);
        pc.onFootstep.AddListener(FootstepEffects);
        pc.onSwimStroke.AddListener(SwimStrokeEffects);
        pc.onWaterEnter.AddListener(WaterEnterEffects);
        pc.onWaterLeave.AddListener(WaterLeaveEffects);
        pc.onMove.AddListener(WaterChangeMovementEffects);

    }

    private void OnDisable()
    {
        pc.onAttack.RemoveListener(AttackEffect);
        pc.onJump.RemoveListener(JumpEffect);
        pc.onLand.RemoveListener(LandEffect);
        en.onHit.RemoveListener(HitEffect);
        pc.onFootstep.RemoveListener(FootstepEffects);
        pc.onSwimStroke.RemoveListener(SwimStrokeEffects);
        pc.onWaterEnter.RemoveListener(WaterEnterEffects);
        pc.onWaterLeave.RemoveListener(WaterLeaveEffects);
        pc.onMove.RemoveListener(WaterChangeMovementEffects);
    }

    #region Footstep Particles

    //Called from the player's walking animation directly
    public void FootstepEffects(bool isOuterFoot)
    {
        SurfaceTypeEntry surfaceTypeEntry = surfaceTypeDetector.GetSurfaceType(); //yes IK this sucks, I can't pass a label for a labeled parameter, FMOD is great :)

        //TODO: early exit for non-dusty non-water case

        //Get footstep particle offset position
        var position = GetParticlePositionOffset(walkFootstepOffsets, isOuterFoot);
                                        
        // Debug.Log($"[PlayerEffects]: Using footstep position {position} for moving input {pc.ViewDirection} and facing left {pv.IsFacingLeft}");
        if(surfaceTypeEntry.type == SurfaceType.Water)
        {
            waterStepParticles.transform.localPosition = position;
            waterStepParticles.Play();
        }
        else //obstacles or terrain, TODO: check dustyness
        {
            var dustcolor = surfaceTypeEntry.dustColor;
            dustcolor.a *= RegionManager.Instance.currentDustiness;
            if (dustcolor.a >= .1f) //skip if dust is invisible anyways
            {
                var main = dustParticles.main;
                main.startColor = dustcolor;
                dustParticles.transform.localPosition = position;
                dustParticles.Play();
            }
        }
        //always play footstep audio regardless
        PlayerSounds.Instance.PlayFootstep(surfaceTypeEntry.type);
    }

    //Called from the player's walking animation directly
    public void SwimStrokeEffects(bool isOuterFoot)
    {
        swimStrokeParticles.transform.localPosition = GetParticlePositionOffset(swimFootstepOffsets, isOuterFoot);
        swimStrokeParticles.Play();
        PlayerSounds.Instance.PlaySwimStroke(); //only play these half of the time
    }


    //No please Tim do not look at this I overcomplicated it again, just appreciate how fancy it looks :)
    //IsOuterFoot only used for footstep particles
    private Vector3 GetParticlePositionOffset(ParticleOffsets offsets, bool isOuterFoot)
    {
        return new Vector3(//moving offset
                                    (Mathf.Abs(pc.ViewDirection.x) > 0 ? Mathf.Sign(pc.ViewDirection.x) * offsets.motionXoffset : 0) + 
                                    //facing direction offset
                                    (pv.IsFacingLeft ? -offsets.viewXoffset : offsets.viewXoffset) + 
                                    //foot offset
                                    (isOuterFoot ^ pv.IsFacingLeft ? offsets.stepSideXOffset : -offsets.stepSideXOffset), 
                    
                                    0, 

                                    (Mathf.Abs(pc.ViewDirection.z) > 0 ? Mathf.Sign(pc.ViewDirection.z) * offsets.motionZoffset : 0));
    }

    #endregion

    #region Water

    public void WaterEnterEffects(float entrySpeed)
    {
        wakeTrail.targetLocalPosition = GetParticlePositionOffset(waterWakeOffsets, false);
        wakeTrail.SetSwimming(true);


        waterEnterParticles.Play();
        if(entrySpeed >= minWaterSplashVelocity) 
        {
            waterSplashParticles.Play();
            PlayerSounds.Instance.PlayWaterSplash();
        }
        else PlayerSounds.Instance.PlayWaterEnter();
    }
    public void WaterLeaveEffects()
    {
        wakeTrail.SetSwimming(false);

        PlayerSounds.Instance.PlayWaterLeave();
    }

    //is hooked up to OnMove()
    public void WaterChangeMovementEffects(Vector3 inputVector)
    {
        if(wakeTrail.IsEmitting()) //only when in water, yes is a bit of hack, nobody actually tracks this apart from the player's animator
        {
            wakeTrail.targetLocalPosition = GetParticlePositionOffset(waterWakeOffsets, false);
            if(Vector3.Magnitude(inputVector) == 0)
            {
                wakeTrail.FadeOutTail();
                waterEnterParticles.Play(); //eliminates the awkward stopping of movement a bit
            }
        }
    }


    #endregion

    private void AttackEffect()
    {
        attackParticles.transform.rotation = Quaternion.LookRotation(pc.ViewDirection, Vector3.up);
        attackParticles.Play();
    }

    private void JumpEffect()
    {
        jumpParticles.Play();
    }

    private void HitEffect(float damage)
    {
        float intensity = Mathf.Clamp01(damage / hitParticlesDamageSaturation);
        var burst = hitParticles.emission.GetBurst(0);
        burst.cycleCount = Mathf.CeilToInt(hitParticlesSaturationCount * intensity); // scale particles by damage
        hitParticles.emission.SetBurst(0, burst);
        hitParticles.time = 0f;
        hitParticles.Play();
    }

    private void LandEffect()
    {
        float speed = rb.linearVelocity.magnitude;
        //print(speed);
        SurfaceTypeEntry surfaceTypeEntry = surfaceTypeDetector.GetSurfaceType();

        Color color;
        switch (surfaceTypeEntry.type)
        {
            case SurfaceType.Grass:
                color = new Color(0.1f, 0.41f, 0.11f);
                break;
            case SurfaceType.Gravel:
                color = new Color(0.545f, 0.271f, 0.075f);
                break;
            case SurfaceType.Wood:
                color = new Color(0.627f, 0.322f, 0.176f);
                break;
            case SurfaceType.Rock:
                color = new Color(0.5f, 0.5f, 0.5f);
                break;
            case SurfaceType.Water:
                color = new Color(0.93f, 0.93f, 0.93f);
                break;
            case SurfaceType.Sand:
                color = new Color(0.941f, 0.902f, 0.549f);
                break;
            default:
                color = Color.white;
                break;
        }
        var mainParticles = landParticles.main;
        mainParticles.startColor = color;
        landParticles.emission.SetBurst(0, new ParticleSystem.Burst(0f, landParticlesSaturationCount * landParticleSpeedVSParticlecountCurve.Evaluate(speed / landParticlesSaturationSpeed)));
        landParticles.Play();
        FootstepEffects(true); //will redo the raycast... uhhhh TODO I guess ~Lars
    }
}
