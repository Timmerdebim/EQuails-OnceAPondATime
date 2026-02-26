using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class WorldItem : MonoBehaviour, IInteractable
{
    public Item item;
    public UnityEvent onPickup;

    void Awake()
    {
        Initialize(item);
    }

    public void Initialize(Item data)
    {
        item = data;
        if (item != null)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = item.GetSprite();
        }

        // Ground the item (TODO) (maybe do with rigid body?) (maybe check if an item can be dropped here?)
        // hoverSequence = DOTween.Sequence()
        // .Append(transform.DOJump())
        // .OnComplete(() => OnGrounded());
        seqRot?.Kill();
        seqRot = DOTween.Sequence()
        .Append(transform.DORotate(-10 * Vector3.forward, 0.5f).SetEase(Ease.OutSine))
        .OnComplete(() => Animate());
    }

    private Sequence seqPos;
    private Sequence seqRot;

    public void Animate()
    {
        float seqPosDuration = 3.1457f; // Try to avoid the animations syncing up (looks weird if change direction and rotation at same time)
        seqPos?.Kill();
        seqPos = DOTween.Sequence()
        .SetLoops(-1, LoopType.Yoyo)
        .Append(transform.DOMove(transform.position + 0.3f * Vector3.up, seqPosDuration / 2).SetEase(Ease.InOutSine));

        float seqRotDuration = 5;
        seqRot?.Kill();
        seqRot = DOTween.Sequence()
        .SetLoops(-1, LoopType.Yoyo)
        .Append(transform.DORotate(10 * Vector3.forward, seqRotDuration / 2).SetEase(Ease.InOutSine));
    }

    #region  Interactable Interface

    virtual public void Interact(UnityAction StopInteractionCallback)
    {
        if (item.TryPickup())
        {
            // onPickup?.Invoke();
            Destroy(gameObject);
        }
    }

    public void StopInteract()
    {
        // None
    }

    public void Focus()
    {
        // Todo: Highlight
    }

    public void Unfocus()
    {
        // Todo: De-Highlight
    }

    #endregion

    private void OnDestroy()
    {
        seqPos?.Kill();
        seqRot?.Kill();
    }
}