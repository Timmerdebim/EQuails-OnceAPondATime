using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GeneralThemeRegion : MonoBehaviour
{
    private BoxCollider col;

    [SerializeField] private FMODUnity.EventReference theme;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.AddTheme(theme);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.RemoveTheme(theme);
        }
    }
}
