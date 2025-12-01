using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Animator animator;

    private int contactCount = 0;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore player or other things you don’t consider ground
        if (other.CompareTag("Player")) return;

        contactCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) return;

        contactCount--;
        if (contactCount < 0) contactCount = 0; // safety
    }

    void Update()
    {
        bool grounded = contactCount > 0;
        animator.SetBool("isGrounded", grounded);
    }
}
