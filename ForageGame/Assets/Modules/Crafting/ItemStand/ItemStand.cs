using Modules.Utils.Nearby;
using System.Collections.Generic;
using UnityEngine;

public class ItemStand : MonoBehaviour
{
    private WorldItem heldItem;
    [SerializeField] bool doAnimation;
    [SerializeField] AnimationCurve rotationAnimationCurve;
    [SerializeField] AnimationCurve heightAnimationCurve;
    [SerializeField] float baseHeight;

    [SerializeField] float suckRadius = 1f;
    private NearbyList<WorldItem> nearbyItems;
    private const float pickupDistance = 0.1f; // Minimum distance to pop the item into the stand
    private const float suckForce = 1f;

    private void Awake()
    {
        nearbyItems = new NearbyList<WorldItem>(0.1f, transform, suckRadius, LayerMask.GetMask("Pickup"));
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
                float forceFactor = (1 / (distance / suckRadius) ) * suckForce; //suck force divided by normalised distance
                
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
    }

    private void Animate()
    {
        float time = Time.time;
        float rotationAngle = rotationAnimationCurve.Evaluate(time % rotationAnimationCurve.keys[rotationAnimationCurve.length - 1].time) * 360f;
        float heightOffset = heightAnimationCurve.Evaluate(time % heightAnimationCurve.keys[heightAnimationCurve.length - 1].time);

        heldItem.transform.localPosition = new Vector3(0, baseHeight + heightOffset, 0);
        heldItem.transform.localRotation = Quaternion.Euler(0, rotationAngle, 0);
    }

    private void RemoveItem()
    {
        heldItem = null;
    }
}
