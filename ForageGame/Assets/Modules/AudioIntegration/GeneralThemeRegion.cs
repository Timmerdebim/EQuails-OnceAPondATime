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
    [HideInInspector] public Coroutine waitRoutine;


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
            if(waitRoutine != null) StopScheduling();
        }
    }

    public void StartScheduling()
    {
        waitRoutine = StartCoroutine(ThemeWaitRoutine());
    }

    public void StopScheduling()
    {
        StopCoroutine(waitRoutine);
        print("Theme scheduling stopped for: " + theme);
    }

    private IEnumerator ThemeWaitRoutine()
    {
        while (true)
        {
            //do not play a theme on cooldown
            float elapsedCooldown = Time.time - lastPlayedTime;
            if (elapsedCooldown < cooldown)
            {
                print("Theme on cooldown: " + theme);
                yield return new WaitForSeconds(cooldown - elapsedCooldown);
                continue;
            }

            print("Now scheduling theme: " + theme);

            float waitTime = Mathf.Max(0.1f, UnityEngine.Random.Range(meanDelay - variance, meanDelay + variance));
            yield return new WaitForSeconds(waitTime);

            print("theme play attempt: " + theme);

            if (MusicManager.Instance.PlayTheme(this))
            {
                lastPlayedTime = Time.time;
            }
            yield break; //end coroutine, but should be stopped by the MusicManager anyway ~Lars
        }
    }
}
