using DG.Tweening;
using Modules.Utils.Nearby;
using System.Collections.Generic;
using UnityEngine;

public class ItemStand : MonoBehaviour
{
    private WorldItem heldItem;
    private float instanceRandomOffset;

    [SerializeField] float suckRadius = 1f;
    private NearbyList<WorldItem> nearbyItems;
    private const float pickupDistance = 0.1f; // Minimum distance to pop the item into the stand
    [SerializeField] private float suckForce = 1f;

    private void Awake()
    {
        nearbyItems = new NearbyList<WorldItem>(0.1f, transform, suckRadius, LayerMask.GetMask("Pickup"));
        instanceRandomOffset = Random.Range(0f, 2 * Mathf.PI);
    }
}
