using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Pickup_Interactable))]
public class Pickup : MonoBehaviour
{
    public string title { get; private set; }
    public string description { get; private set; }
    public Graphic icon { get; private set; }
    public bool isConsumable { get; private set; }
    [SerializeField] private float consumableEnergy;
    private Pickup_Interactable pickupInteractable;
    public bool isBusy { get; private set; } = false;
    private Transform originalParent;
    [SerializeField] private float lerpDuration = 1f;

    // ALL FUNCTIONS SHOULD ONLY BE CALLED BY THE INVENTORY SCRIPT!!!

    void Start()
    {
        isBusy = false;
        originalParent = transform.parent;
        pickupInteractable = GetComponent<Pickup_Interactable>();
    }

    public void PickupItem(Vector2 targetPos)
    {
        Debug.Log(5);
        // To be called ONLY by the inventory
        isBusy = true;                                  // Set to busy
        pickupInteractable.enabled = false;             // first disable world state
        SetupAsUIElement();
        StartCoroutine(AnimateToHotbar(targetPos)); // next animate; after set to not busy
    }

    public void ConsumeItem()
    {
        // To be called ONLY by the inventory
        // TODO: Change player energy amount
        Destroy(this);
    }

    public void TossItem(Vector3 targetPos)
    {
        // To be called ONLY by the inventory
        isBusy = true;                              // Set to busy
        SetupAsWorldObject();
        StartCoroutine(AnimateToWorld(targetPos));  // next animate; after set to not busy AND enable world state once complete
    }

    private void SetupAsUIElement()
    {
        // Change parent to canvas
        transform.SetParent(Inventory.Instance.canvas.transform, true);
        // Set as last sibling to render on top
        transform.SetAsLastSibling();
        // Enable UI components
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) rectTransform = gameObject.AddComponent<RectTransform>();
    }

    private void SetupAsWorldObject()
    {
        // Reset parent
        transform.SetParent(originalParent, true);
        // Disable UI
        // Remove RectTransform if it was added
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null) Destroy(rectTransform);
    }

    // ----------------------- The following are the animations ------------------------------------------------------------------

    Vector3 animationJump3D = new Vector3(0f, 0f, 1f);
    Vector2 animationJump2D = new Vector2(0f, 1f);

    private System.Collections.IEnumerator AnimateToHotbar(Vector2 targetPos)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        isBusy = true;
        float animationTime = 0f;

        // Store start values
        Vector2 startPos = rectTransform.position;
        Vector2 startScale = rectTransform.localScale;

        while (animationTime < lerpDuration)
        {
            animationTime += Time.deltaTime;
            float t = SmoothLerp(animationTime / lerpDuration);

            // Interpolate position
            rectTransform.anchoredPosition = (1 - t) * startPos + t * targetPos + Mathf.Pow(2 * t - 1, 2) * animationJump2D;
            // Interpolate scale (optional - scale to UI size)
            // transform.localScale = Vector2.Lerp(startScale, 1, t);

            yield return null;
        }
        // Animation complete - setup as UI element
        isBusy = false;
    }

    private System.Collections.IEnumerator AnimateToWorld(Vector3 targetPos)
    {
        isBusy = true;
        float animationTime = 0f;
        // Store start values
        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;

        while (animationTime < lerpDuration)
        {
            animationTime += Time.deltaTime;
            float t = SmoothLerp(animationTime / lerpDuration);

            // Interpolate position
            transform.position = (1 - t) * startPos + t * targetPos + Mathf.Pow(2 * t - 1, 2) * animationJump3D;
            // Interpolate scale back to original
            // transform.localScale = Vector3.Lerp(startScale, 1, t);

            yield return null;
        }
        // Animation complete
        isBusy = false;
    }

    private float SmoothLerp(float t)
    {
        return 3 * t * t - 2 * t * t * t;
    }
}

