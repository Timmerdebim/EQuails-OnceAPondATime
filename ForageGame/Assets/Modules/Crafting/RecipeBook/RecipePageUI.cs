using UnityEngine;
using UnityEngine.UI;

public class RecipePageUI : MonoBehaviour
{
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayFlipLeftAnim()
    {
        animator.SetTrigger("FlipLeft");
    }

    public void PlayFlipRightAnim()
    {
        animator.SetTrigger("FlipRight");
    }
}
