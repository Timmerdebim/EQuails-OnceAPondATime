using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class GeneralThemeRegion : MonoBehaviour
{
    private BoxCollider col;

    [SerializeField] public FMODUnity.EventReference theme;  // FMOD event path
    [SerializeField] private float meanDelay;
    [SerializeField] private float variance;
    [SerializeField] private float cooldown;

    [HideInInspector] public float lastPlayedTime = -Mathf.Infinity; //for theme cooldown


    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.AddTheme(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.RemoveTheme(this);
        }
    }

    public float GetScheduledTime()
    {
        print("Theme scheduled for time: " + Time.time + meanDelay);
        return Time.time + meanDelay; //TODO: change to have the distro
    }

}
