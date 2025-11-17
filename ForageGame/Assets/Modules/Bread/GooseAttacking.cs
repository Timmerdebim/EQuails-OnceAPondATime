using UnityEngine;

public class GooseAttacking : StateMachineBehaviour {
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
    private Vector3 lungeTargetPosition;

    // An enum to clearly manage which part of the attack we're in.
    private enum AttackPhase { WindUp, Lunge, Cooldown }
    private AttackPhase currentPhase;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        goose = animator.GetComponent<BigGoose>();
        gooseTransform = animator.transform;
        
        if (goose == null) {
            Debug.LogError("GooseAttacking: No BigGoose found on " + animator.gameObject.name);
            return;
        }

        // Stop all previous movement cleanly using the goose's utility function.
        goose.StopNavMovement();
        goose.attackHitbox.enabled = false;

        // Initialize the attack state
        timer = 0f;
        currentPhase = AttackPhase.WindUp;

        // Immediately face the target to begin the telegraph
        if (goose.closestDuck != null) {
            Vector3 directionToDuck = (goose.closestDuck.transform.position - gooseTransform.position).normalized;
            goose.rotation = Quaternion.LookRotation(new Vector3(directionToDuck.x, 0, directionToDuck.z));
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
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
                // wait cooldownDuration seconds before transitioning back to wind-up
                // if (timer >= cooldownDuration)
                // {
                //     timer = 0f;
                //     currentPhase = AttackPhase.WindUp;
                //     
                // }
                // // // Immediately face the target to begin the telegraph
                // Vector3 directionToDuck = (goose.closestDuck.transform.position - gooseTransform.position).normalized;
                // goose.rotation = Quaternion.LookRotation(new Vector3(directionToDuck.x, 0, directionToDuck.z));
                // goose.transform.rotation = goose.rotation;
                break;
        }
    }

    private void ProcessWindUp()
    {
        // Continue to face the duck during the wind-up to show focus
        Vector3 directionToDuck = (goose.closestDuck.transform.position - gooseTransform.position).normalized;
        goose.rotation = Quaternion.LookRotation(new Vector3(directionToDuck.x, 0, directionToDuck.z));

        // When the wind-up time is over, transition to the lunge
        if (timer >= windUpDuration)
        {
            timer = 0f;
            currentPhase = AttackPhase.Lunge;
            
            // IMPORTANT: We lock the target position here. This makes the attack dodgeable.
            lungeTargetPosition = goose.closestDuck.transform.position;
            goose.attackHitbox.enabled = true; // Activate the attack!
        }
    }

    private void ProcessLunge()
    {
        // Move the goose forward in the direction it's facing
        // This creates the lunge effect.
        gooseTransform.position += gooseTransform.forward * lungeSpeed * Time.deltaTime;

        // When the lunge time is over, transition to cooldown
        if (timer >= lungeDuration)
        {
            timer = 0f;
            currentPhase = AttackPhase.Cooldown;
            goose.attackHitbox.enabled = false; // Deactivate the attack.
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // As a safety measure, always ensure the hitbox is disabled when exiting this state.
        if (goose != null)
        {
            goose.attackHitbox.enabled = false;
        }
    }
}