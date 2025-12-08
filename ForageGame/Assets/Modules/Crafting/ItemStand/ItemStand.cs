using DG.Tweening;
using Modules.Utils.Nearby;
using System.Collections.Generic;
using UnityEngine;

public class ItemStand : MonoBehaviour
{
    private WorldItem heldItem;
    [SerializeField] bool doAnimation;
    [SerializeField] float animationSpeed;
    [SerializeField] float heightAnimationAmplitude;
    [SerializeField] float rotationAnimationAmplitude;
    [SerializeField] float baseHeight;
    private float instanceRandomOffset;

    [SerializeField] float suckRadius = 1f;
    private NearbyList<WorldItem> nearbyItems;
    private const float pickupDistance = 0.1f; // Minimum distance to pop the item into the stand
    [SerializeField]private float suckForce = 1f;



    private void Awake()
    {
        nearbyItems = new NearbyList<WorldItem>(0.1f, transform, suckRadius, LayerMask.GetMask("Pickup"));
        instanceRandomOffset = Random.Range(0f, 2*Mathf.PI);
    }

    private void Update()
    {
        if(heldItem == null)
        {
            SuckNearbyItems();
        }
        else if(doAnimation)
        {
            Animate();
        }
    }

    public WorldItem GetHeldItem()
    {
        if( heldItem == null ) { return null; }
        return heldItem;
    }

    private void SuckNearbyItems()
    {
        //if(nearbyItems == null) { return; }
        List<WorldItem> nearbyItems = NearbyUtil<WorldItem>.GetNearbyObjects(transform.position, suckRadius, LayerMask.GetMask("Pickup"));
        foreach(WorldItem item in nearbyItems)
        {
            if (item == null) continue;
            float distance = Vector3.Distance(item.transform.position, transform.position);
            if (distance <= pickupDistance)
            {
                PlaceItem(item);
                break;
            }

            if (distance <= suckRadius)
            {
                Vector3 dir = (transform.position - item.transform.position).normalized;
                float forceFactor = (1 / (distance / suckRadius + 1) ) * suckForce; //suck force divided by normalised distance
                
                item.transform.Translate(dir * forceFactor);
            }
        }
    }

    public void PlaceItem(WorldItem item)
    {
        heldItem = item;
        heldItem.onPickup.AddListener(RemoveItem);
        heldItem.transform.SetParent(transform);

        heldItem.transform.localPosition = new Vector3(0, baseHeight, 0);
        heldItem.transform.localScale = Vector3.zero;
        heldItem.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutBack);

    }

    private void Animate()
    {
        float time = Time.time + instanceRandomOffset;
        float rotationAngle = rotationAnimationAmplitude * Mathf.Sin(1.1f * time * animationSpeed);
        float heightOffset = heightAnimationAmplitude * transform.lossyScale.y * Mathf.Sin(time * animationSpeed);

        heldItem.transform.localPosition = new Vector3(0, baseHeight + heightOffset, 0);
        heldItem.transform.localRotation = Quaternion.Euler(0, 0, rotationAngle);
    }

    public void RemoveItem()
    {
        heldItem = null;
    }
}
