using UnityEngine;

[ExecuteInEditMode]
public class GhostSync : MonoBehaviour
{
    private SpriteRenderer _parentRenderer;
    private SpriteRenderer _myRenderer;

    void OnEnable()
    {
        _myRenderer = GetComponent<SpriteRenderer>();
        // Assumes this object is a direct child of the main character
        if (transform.parent != null)
            _parentRenderer = transform.parent.GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (_parentRenderer && _myRenderer)
        {
            // Sync the sprite frame and flip state
            _myRenderer.sprite = _parentRenderer.sprite;
            _myRenderer.flipX = _parentRenderer.flipX;
            _myRenderer.flipY = _parentRenderer.flipY;
        }
    }
}