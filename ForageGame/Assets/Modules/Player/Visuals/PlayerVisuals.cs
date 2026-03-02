using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(SpriteLibrary))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisuals : MonoBehaviour
{
    private SpriteLibrary spriteLibrary;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteLibraryAsset[] spriteLibraryAssets = new SpriteLibraryAsset[8];
    // 0, 2, 4, 6 are the wing levels
    // 0, 1 are back and front
    private bool isFacingLeft = true;
    private bool isFacingFront = true;
    private int wingLevel = 0;

    void Awake()
    {
        spriteLibrary = GetComponent<SpriteLibrary>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateVisuals()
    {
        UpdateWingVisuals();
        UpdateViewVisuals();
    }

    public void UpdateWingVisuals()
    {
        int oldWingLevel = wingLevel;
        wingLevel = Player.Instance.playerData.wingLevel;
        if (oldWingLevel == wingLevel) return;
        SetSpriteLibraryAsset();
    }

    public void UpdateViewVisuals()
    {
        UpdateViewVisualsX();
        UpdateViewVisualsZ();
    }

    public void UpdateViewVisualsX()
    {
        bool oldIsFacingLeft = isFacingLeft;
        if (Player.Instance.playerController.ViewDirection.x < 0) isFacingLeft = true;
        else if (Player.Instance.playerController.ViewDirection.x > 0) isFacingLeft = false;
        // else if == 0, we do nothing as not to snap the player around
        if (oldIsFacingLeft == isFacingLeft) return;
        spriteRenderer.flipX = !isFacingLeft;
    }

    public void UpdateViewVisualsZ()
    {
        bool oldIsFacingFront = isFacingFront;
        isFacingFront = Player.Instance.playerController.ViewDirection.z <= 0;
        // We do snap the player to face forward if given the option
        if (oldIsFacingFront == isFacingFront) return;
        SetSpriteLibraryAsset();
    }

    private void SetSpriteLibraryAsset()
    {
        int index = 2 * Math.Clamp(wingLevel, 0, 3);
        index += isFacingFront ? 1 : 0;
        spriteLibrary.spriteLibraryAsset = spriteLibraryAssets[index];
    }
}
