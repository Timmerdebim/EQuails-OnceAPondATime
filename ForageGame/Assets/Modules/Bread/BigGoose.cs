using UnityEngine;

public class BigGoose : MonoBehaviour {
    private static readonly int ClosestDuckDist = Animator.StringToHash("closestDuckDist");

    public Animator animator;
    public HealthComponent healthComponent;
    
    public float chaseSpeed = 5f;
    
    // tracking
    public float closestDuckDist = float.MaxValue;
    public DuckController closestDuck = null;
    
    public Vector3 velocity = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        healthComponent = GetComponent<HealthComponent>();
        if (healthComponent == null) {
            Debug.LogError("Bread: No HealthComponent found on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update() {
        // geese chase ducks
        float frameClosestDuckDist = float.MaxValue;
        DuckController frameClosestDuck = null;
        foreach (DuckController duck in FindObjectsByType<DuckController>(FindObjectsSortMode.None)) {
            float dist = Vector3.Distance(duck.transform.position, transform.position);
            if (dist < frameClosestDuckDist) {
                frameClosestDuckDist = dist;
                frameClosestDuck = duck;
            }
        }
        closestDuckDist = frameClosestDuckDist;
        closestDuck = frameClosestDuck;
        animator.SetFloat(ClosestDuckDist, closestDuckDist);
        // print
        // Debug.Log("closestDuckDist: " + closestDuckDist);
        
        // apply velocity and rotation
        transform.position += velocity * Time.deltaTime;
        transform.rotation = rotation;
    }
}