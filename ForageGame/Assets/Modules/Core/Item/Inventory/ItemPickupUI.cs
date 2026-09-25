using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;

namespace TDK.ItemSystem.Inventory
{
    public class ItemPickupUI : MonoBehaviour
    {
        [Header("UI References")]
        public Image itemIcon;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemDescription;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void TriggerNewItemPopup(ItemData item)
        {
            // pause game
            Time.timeScale = 0f;
            gameObject.SetActive(true);

            transform.DOScale(Vector3.one, 0.4f).From(Vector3.zero).SetEase(Ease.OutBack).SetUpdate(true);

            itemIcon.sprite = item.GetSprite();
            itemName.text = item.GetName();
            itemDescription.text = item.GetDescription();
            StartCoroutine(ShowPopup());
        }

        public IEnumerator ShowPopup()
        {
            // Optional small delay so player can't instantly skip
            yield return new WaitForSecondsRealtime(0.3f);

            // Wait for any key
            while (!Input.anyKeyDown)
                yield return null;

            // Resume game
            Time.timeScale = 1f;
            transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true).onComplete = () => gameObject.SetActive(false);
        }
    }
}