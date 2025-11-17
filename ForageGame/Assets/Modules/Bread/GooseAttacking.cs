using UnityEngine;

public class GooseAttacking : StateMachineBehaviour
{
    [Header("Attack Timings")]
    [Tooltip("How long the goose waits and stares at the target before lunging.")]
    public float windUpDuration = 0.7f;
    [Tooltip("How long the actual lunge and attack lasts.")]
    public float lungeDuration = 0.3f;
    [Tooltip("How long the goose is stuck in recovery after the lunge.")]
    public float cooldownDuration = 1.0f;

    [Header("Attack Properties")]
    [Tooltip("How fast the goose moves forward during the lunge.")]
    public float lungeSpeed = 15f;

    // Private state variables
    private BigGoose goose;
    private Transform gooseTransform;
    private float timer;

    // An enum to clearly manage which part of the attack we're in.
    private enum AttackPhase { WindUp, Lunge, Cooldown }
    private AttackPhase currentPhase;

    // OnStateEnter is called when a transition starts
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        goose = animator.GetComponent<BigGoose>();
        gooseTransform = animator.transform;
        if (goose == null)
        {
            Debug.LogError("GooseAttacking: No BigGoose found on " + animator.gameObject.name);
            return;
        }

        // Stop all previous movement and disable agent-controlled rotation for the attack.
        goose.StopNavMovement();
        goose.navMeshAgent.updateRotation = false; 
        goose.attackHitbox.enabled = false;

        // Initialize the attack state
        timer = 0f;
        currentPhase = AttackPhase.WindUp;
        
        // Immediately face the target to begin the telegraph
        if (goose.closestDuck != null)
        {
            Vector3 directionToDuck = (goose.closestDuck.transform.position - gooseTransform.position).normalized;
            goose.rotation = Quaternion.LookRotation(new Vector3(directionToDuck.x, 0, directionToDuck.z));
        }
    }

    // OnStateUpdate is called on each Update frame
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If goose or the duck is gone, do nothing. The animator will transition out.
        if (goose == null || goose.closestDuck == null) return;

        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case AttackPhase.WindUp:
                ProcessWindUp();
                break;
            case AttackPhase.Lunge:
                ProcessLunge();
                break;
            case AttackPhase.Cooldown:
                // During cooldown, the goose does nothing but wait.
                // The state will exit via an Animator transition when the animation clip finishes.

                break;
        }
    }

    private void ProcessWindUp()
    {
        // Continue to face the duck during the wind-up to show focus
        Vector3 directionToDuck = (goose.closestDuck.transform.position - gooseTransform.position).normalized;
        goose.rotation = Quaternion.LookRotation(new Vector3(directionToDuck.x, 0, directionToDuck.z));

        if (timer >= windUpDuration)
        {
            timer = 0f;
            currentPhase = AttackPhase.Lunge;
            goose.attackHitbox.enabled = true; // Activate the attack!
        }
    }

    private void ProcessLunge()
    {
        // Manually move the goose forward. This creates the lunge effect.
        gooseTransform.position += gooseTransform.forward * lungeSpeed * Time.deltaTime;

        if (timer >= lungeDuration)
        {
            timer = 0f;
            currentPhase = AttackPhase.Cooldown;
            goose.attackHitbox.enabled = false; // Deactivate the attack.
        }
    }

    // OnStateExit is called when a transition ends
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (goose != null)
        {
            // Safety measure: always disable hitbox when exiting.
            goose.attackHitbox.enabled = false;
            goose.navMeshAgent.updateRotation = true; 
        }
    }
}