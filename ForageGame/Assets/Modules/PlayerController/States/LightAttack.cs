using UnityEngine;

public class LightAttack : Idle {
    // private DuckController duck;
    private float attackTime;
    private float timeSinceAttackStart;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // get gameobject this is attached to
        GameObject obj = animator.gameObject;
        // get duck controller
        duck = obj.GetComponent<DuckController>();
        if (duck.attackType == DuckController.AttackType.light) {
            attackTime = duck.lightAttackTime;
            // duck.animator.Play("LightAttack", 0, 0);
        } else if (duck.attackType == DuckController.AttackType.heavy) {
            attackTime = duck.heavyAttackTime;
            // duck.animator.Play("HeavyAttack", 0, 0);
        }
        
        duck.animator.SetInteger("attackType", 1);

        timeSinceAttackStart = 0;
        // duck.attacking = false; // reset input
        // Debug.Log("entering attack state");
        duck.hitboxCollider.enabled = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeSinceAttackStart += Time.deltaTime;
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        // NextState();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duck.attackType = DuckController.AttackType.none;
        duck.attacking = false;
        duck.hitboxCollider.enabled = false;
        duck.animator.SetInteger("attackType",  0);  
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
