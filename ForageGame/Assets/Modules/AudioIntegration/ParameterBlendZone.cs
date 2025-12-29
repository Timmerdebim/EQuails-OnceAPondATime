using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider))]
public class ParameterBlendZone : MonoBehaviour
{
    [Header("Blend Curves")]
    public AnimationCurve aCurve = AnimationCurve.Linear(0, 1, 1, 0);
    public AnimationCurve bCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private BoxCollider col;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        if (!col.isTrigger)
            Debug.LogWarning("Collider should be a trigger!");
    }

    //basically an update() while there is a collision
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Vector3 localPos = transform.InverseTransformPoint(other.transform.position); //local position, so we can also rotate the box

            float halfLength = col.size.z * 0.5f;
            float progress01 = Mathf.InverseLerp(-halfLength, halfLength, localPos.z); //how far are we in the length

            // Evaluate curves, TODO: do stuff lol
            float valueA = aCurve.Evaluate(progress01);
            float valueB = bCurve.Evaluate(progress01);
            ApplyBlend(valueA, valueB);
        }
    }

    //This is debug
    void ApplyBlend(float A, float B)
    {
        Debug.Log($"Blend A:{A:F2}, B:{B:F2}");
    }
}
