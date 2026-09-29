using UnityEngine;
using TDK.ItemSystem.Inventory;
using TDK.ItemSystem.Types;
using TDK.ItemSystem;
using TDK.PlayerSystem;
using NPC;

public class InGameHints : MonoBehaviour
{
    private static readonly int RunHash = Animator.StringToHash("run");
    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private float idleTime = 0;
    private readonly float activationTime = 1.5f;

    void Update()
    {
        if (Input.anyKey) idleTime = 0;
        else idleTime += Time.deltaTime;

        RefreshCurrentHints();
    }

    [Header("Built In Help Prompts")]
    [SerializeField] private InGameHintElement consumeHint;
    [SerializeField] private InGameHintElement pickupHint;
    [SerializeField] private InGameHintElement talkHint;
    [SerializeField] private InGameHintElement attackHint;
    [SerializeField] private InGameHintElement dashHint;
    [SerializeField] private InGameHintElement jumpHint;
    [SerializeField] private InGameHintElement flyHint;
    [SerializeField] private InGameHintElement recipeHint;
    [SerializeField] private InGameHintElement nextHint;
    [SerializeField] private InGameHintElement previousHint;
    [SerializeField] private InGameHintElement dropHint;

    public void RefreshCurrentHints()
    {
        pickupHint.SetActive(!IsBookOpen && Player.Instance.playerInteract._currentFocus != null && Player.Instance.playerInteract._currentFocus.GetComponent<ItemController>() != null);
        talkHint.SetActive(!IsBookOpen && Player.Instance.playerInteract._currentFocus != null && Player.Instance.playerInteract._currentFocus.GetComponent<NpcLocation>() != null);
        dropHint.SetActive(!IsBookOpen && InventoryController.Instance.GetItemAtCurrent() != null && HasTimePassed);

        dashHint.SetActive(!IsBookOpen && Player.Instance._playerAnimator._animator.GetBool(IsMovingHash) && !Player.Instance._playerAnimator._animator.GetBool(RunHash));

        consumeHint.SetActive(!IsBookOpen && InventoryController.Instance.GetItemAtCurrent() is ConsumableItem && (HasTimePassed || !Player.Instance.playerData.hasConsumedItem));
        attackHint.SetActive(!IsBookOpen && Player.Instance.playerData.attackUnlocked && (HasTimePassed || !Player.Instance.playerData.hasUsedAttack));
        jumpHint.SetActive(!IsBookOpen && Player.Instance.playerData.wingLevel == 1 && (HasTimePassed || !Player.Instance.playerData.hasUsedJump));
        flyHint.SetActive(!IsBookOpen && Player.Instance.playerData.wingLevel == 2 && (HasTimePassed || !Player.Instance.playerData.hasUsedFly));
        recipeHint.SetActive(!IsBookOpen && RecipeBookController.Instance.CollectedRecipes.Count > 0 && (HasTimePassed || !Player.Instance.playerData.hasOpenedRecipeBook));

        nextHint.SetActive(IsBookOpen);
        previousHint.SetActive(IsBookOpen);
    }

    private bool HasTimePassed { get => idleTime > activationTime; }
    private bool IsBookOpen { get => RecipeBookController.Instance.IsVisualized; }
}
