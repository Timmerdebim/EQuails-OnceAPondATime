using System.Collections;
using UnityEngine;

public class RandomUtils
{
    public float Normal(float mean, float stdDev)
    {
        // Using Box-Muller transform to generate a normally distributed random number
        float u1 = 1.0f - Random.value; // uniform(0,1] random float
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stdDev * randStdNormal;
    }
}