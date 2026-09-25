using UnityEngine;
using UnityEngine.UI;

namespace TDK.ItemSystem.Inventory
{
    public class RecipePageUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public void PlayFlipLeftAnim()
        {
            animator.SetTrigger("FlipLeft");
        }

        public void PlayFlipRightAnim()
        {
            animator.SetTrigger("FlipRight");
        }
    }

}