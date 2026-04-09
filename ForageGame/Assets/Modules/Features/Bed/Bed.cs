using Assets.Modules.Interaction;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    public void Interact() => _ = GameplayController.Instance.Sleep(); // hawk-shew-mi-mi-mi

    public void Focus() { }

    public void Unfocus() { }
}
