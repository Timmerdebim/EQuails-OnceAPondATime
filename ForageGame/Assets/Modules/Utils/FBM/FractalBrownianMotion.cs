using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class FBM1D
{
    Func<float, float> func;
    FBMSettings settings;
    private int octaves { get { return settings.octaves; } }
    private float lacunarity { get { return settings.lacunarity; } }
    private float persistence { get { return settings.persistence; } }

    float _norm = 0f;

    float norm
    {
        get
        {
            if (_norm == 0f)
            {
                _norm = FBM1Dnorm(func, octaves, persistence);
            }
            return _norm;
        }
    }

    /// <summary>
    /// Constructs a 1D fBM function.
    /// </summary>
    /// <param name="func">Base function, e.g. Sin or Perlin Noise.</param>
    /// <param name="octaves">Number of fBM layers.</param>
    /// <param name="lacunarity">The lacunarity of the fBM is the gain of the frequency. Should normally be >1.</param>
    /// <param name="persistence">The persistence of the fBM is the gain of the amplitude. Should normally be <1.</param>
    public FBM1D(Func<float, float> func, int octaves = 4, float lacunarity = 2f, float persistence = 0.5f)
    {
        this.func = func;
        this.settings = new FBMSettings(octaves, lacunarity, persistence);
    }

    public FBM1D(Func<float, float> func, FBMSettings settings)
    {
        this.func = func;
        this.settings = settings;
    }

    [System.Serializable]
    public class FBMSettings
    {
        public float lacunarity;
        public float persistence;
        public int octaves;

        public FBMSettings(int octaves = 4, float lacunarity = 2f, float persistence = 0.5f)
        {
            this.lacunarity = lacunarity;
            this.persistence = persistence;
            this.octaves = octaves;
        }
    }

    /// <summary>
    /// Returns the 1D fBM function value at x.
    /// </summary>
    /// <param name="x">Function argument.</param>
    /// <returns>The 1D fBM function value at x.</returns>
    public float Eval(float x)
    {
        float sum = 0f;

        for (int i = 0; i < octaves; i++)
        {
            sum += Mathf.Pow(persistence, i) * func(Mathf.Pow(lacunarity, i) * x);
        }

        return sum;
    }

    /// <summary>
    /// Returns the normalised 1D fBM function value at x in the range [0, 1].
    /// </summary>
    /// <param name="x">Function argument</param>
    /// <returns>1D fBM function on domain [0, 1]</returns>
    public float Eval01(float x)
    {
        return (Eval(x) / norm + 1f) / 2f;
    }

    /// <summary>
    /// Returns the normalised 1D fBM function value at x in the range [-1, 1].
    /// </summary>
    /// <param name="x">Function argument</param>
    /// <returns>1D fBM function on domain [-1, 1]</returns>
    public float EvalMin11(float x)
    {
        return Eval(x) / norm;
    }

    /// <summary>
    /// Returns the norm of the 1D fBM function for normalisation purposes, using random sampling for the extrema of the function over domain [0,10].
    /// </summary>
    /// <param name="octaves"></param>
    /// <param name="persistence"></param>
    /// <returns></returns>
    private static float FBM1Dnorm(Func<float, float> func, int octaves, float persistence)
    {

        float min = float.PositiveInfinity;
        float max = float.NegativeInfinity;

        for (float x = 0f; x < 10f; x += 0.01f)
        {
            float y = func(x);

            if (y < min) min = y;
            if (y > max) max = y;
        }

        float maxVal = Mathf.Max(Mathf.Abs(min), Mathf.Abs(max));

        float norm = 0f;
        for (int i = 0; i < octaves; i++)
        {
            norm += maxVal * Mathf.Pow(persistence, i);
        }

        return norm;
    }

}