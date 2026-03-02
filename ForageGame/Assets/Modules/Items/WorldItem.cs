using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;
using DG.Tweening;

namespace Project.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class WorldItem : MonoBehaviour, IInteractable
    {
        public Item item;

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
            Animate();
        }


        private Tweener doMove;
        private Tweener doRotate;

        public void MoveTo(Vector3 target, float duration)
        {
            doMove?.Kill();
            doMove = transform.DOBlendableMoveBy(target - transform.position, duration)
            .OnComplete(() => { Animate(); });

            doRotate?.Kill();
            doRotate = transform.DORotate(Vector3.zero, 5 / 4);
        }

        public void Animate()
        {
            float seqPosDuration = Mathf.PI; // Try to avoid the animations syncing up (looks weird if change direction and rotation at same time)
            doMove?.Kill();
            doMove = transform.DOBlendableMoveBy(0.3f * Vector3.up, seqPosDuration / 2)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

            float seqRotDuration = 5;
            doRotate?.Kill();
            doRotate = transform.DORotate(-10 * Vector3.forward, seqRotDuration / 4)
            .OnComplete(() =>
            {
                doRotate?.Kill();
                doRotate = transform.DOBlendableRotateBy(20 * Vector3.forward, seqRotDuration / 2)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            });
        }

        #region  Interactable Interface

        virtual public void Interact()
        {
            if (item.TryWorldItemInteract())
                Destroy(gameObject);
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
            doMove?.Kill();
            doRotate?.Kill();
        }
    }
}