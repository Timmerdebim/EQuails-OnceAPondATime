using UnityEngine;
using Assets.Modules.Interaction;
using UnityEngine.Events;
using DG.Tweening;
using TDK.SaveSystem;

namespace TDK.ItemSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody))]
    public class ItemController : MonoBehaviour, IInteractable, ISaveable
    {
        public ItemData ItemData;

        private Rigidbody _rigidbody;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        void Reset()
        {
            if (ItemData != null)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = ItemData.GetSprite();
            }
        }

        public void Initialize(ItemSaveData data) => Initialize(data.GetItemData(), data.Position, data.Velocity);
        public void Initialize(ItemData item, Vector3 position, Vector3 velocity)
        {
            ItemData = item;
            transform.position = position;
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.linearVelocity = velocity;
            if (ItemData != null)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = item.GetSprite();
            }
        }


        private Tweener doMove;

        public void MoveTo(Vector3 target, float duration)
        {
            doMove?.Kill();
            doMove = transform.DOBlendableMoveBy(target - transform.position, duration);
        }

        #region  Interactable Interface

        virtual public void Interact()
        {
            if (ItemData.TryWorldItemInteract())
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
        }

        public void SaveData(ref WorldSaveData data)
        {
            if (ItemData == null) return;
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            data.Items.Add(new()
            {
                ItemId = ItemData.GetId(),
                Position = transform.position,
                Velocity = rigidbody.linearVelocity
            });
        }
    }
}