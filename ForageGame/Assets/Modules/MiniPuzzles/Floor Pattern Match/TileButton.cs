using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class TileButton : MonoBehaviour
{
    [SerializeField] private FloorPatternMatch FPM;
    [SerializeField] private LayerMask triggeringLayers;
    [HideInInspector] public bool _isUseable = true;
    [SerializeField] private bool targetState = true;
    [HideInInspector] public bool isCorrect = false;
    public bool state { get; private set; } = false;

    [Header("Rendering")]
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private MeshRenderer buttonRenderer;

    void Awake()
    {
        RefreshButton();
    }

    void OnTriggerEnter(Collider other)
    {
        if ((triggeringLayers.value & (1 << other.gameObject.layer)) != 0)
            ButtonClickyUWU();
    }

    private void RefreshButton()
    {
        if (state)
            buttonRenderer.material = onMaterial;
        else
            buttonRenderer.material = offMaterial;

        isCorrect = state == targetState;
    }

    private void ButtonClickyUWU()
    {
        if (!_isUseable)
            return;

        state = !state;

        RefreshButton();

        FPM.UpdatePuzzle();
    }
}
