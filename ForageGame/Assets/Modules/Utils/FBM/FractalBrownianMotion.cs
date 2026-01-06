using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class FBM1D
{
    private struct NoiseFunction
    {
        public (float, float) range;
        public Func<float, float> function;
    }

    public enum NoiseFunctionType
    {
        Sin,
        PerlinNoise1D,
        Sawtooth,
        Square,
        Custom
    }

    private Func<float, float> customFunc;

    private Dictionary<NoiseFunctionType, NoiseFunction> NoiseFunctions = new Dictionary<NoiseFunctionType, NoiseFunction>()
    {
        {NoiseFunctionType.Sin, new NoiseFunction(){range = (-1f, 1f), function = Mathf.Sin } },
        {NoiseFunctionType.PerlinNoise1D, new NoiseFunction(){range = (-1f, 1f), function = Mathf.PerlinNoise1D } },
        {NoiseFunctionType.Sawtooth, new NoiseFunction(){range = (0f, 1f), function = (float x) => x - Mathf.Floor(x) } },
        {NoiseFunctionType.Square, new NoiseFunction(){range = (0f, 1f), function = (float x) => (x - Mathf.Floor(x)) < 0.5f ? 0f : 1f} },
        {NoiseFunctionType.Custom, new NoiseFunction(){} }
    };

    Func<float, float> func
    {
        get
        {
            return NoiseFunctions[settings.funcType].function;
        }
    }

    FBMSettings settings;
    private int octaves { get { return settings.octaves; } }
    private float lacunarity { get { return settings.lacunarity; } }
    private float persistence { get { return settings.persistence; } }


    // Used if the exact range is not known
    float min = float.MaxValue;
    float max = float.MinValue;

    public FBM1D(FBMSettings settings)
    {
        this.settings = new FBMSettings(settings.funcType, octaves, lacunarity, persistence);

        min = NoiseFunctions[settings.funcType].range.Item1 * (1 - Mathf.Pow(persistence, octaves)) / (1 - persistence);
        max = NoiseFunctions[settings.funcType].range.Item2 * (1 - Mathf.Pow(persistence, octaves)) / (1 - persistence);
    }

    public FBM1D(NoiseFunctionType funcType = NoiseFunctionType.Sin, int octaves = 4, float lacunarity = 2f, float persistence = 0.5f) : this(new FBMSettings(funcType, octaves, lacunarity, persistence)) { }

    public FBM1D(FBMSettings settings, Func<float, float> customFunc)
    {
        settings.funcType = NoiseFunctionType.Custom;
        this.settings = settings;

        this.NoiseFunctions[NoiseFunctionType.Custom] = new NoiseFunction() { function = customFunc };

        ApproximateRange((-100f, 100f), 0.1f);
    }


    [System.Serializable]
    public class FBMSettings
    {
        public NoiseFunctionType funcType;
        public float lacunarity;
        public float persistence;
        public int octaves;

        public FBMSettings(NoiseFunctionType func = NoiseFunctionType.Sin, int octaves = 4, float lacunarity = 2f, float persistence = 0.5f)
        {
            this.funcType = func;
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
        return (x - min) / (max - min);
    }

    /// <summary>
    /// Returns the normalised 1D fBM function value at x in the range [-1, 1].
    /// </summary>
    /// <param name="x">Function argument</param>
    /// <returns>1D fBM function on domain [-1, 1]</returns>
    public float EvalMin11(float x)
    {
        return Eval01(x) * 2f - 1f;
    }

    private void ApproximateRange((float,float) Domain, float step)
    {
        for (float x = Domain.Item1; x < Domain.Item2; x += step)
        {
            float val = Eval(x);
            min = Mathf.Min(min, val);
            max = Mathf.Max(max, val);
        }
    }

}