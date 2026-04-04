using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[System.Serializable]
public class FBM1D : ISerializationCallbackReceiver
{
    public NoiseFunctionType funcType;
    public int octaves;
    public float lacunarity;
    public float persistence;

    #region CustomFunction
    // Used for custom functions where the range is not known ahead of time
    float min = float.MaxValue;
    float max = float.MinValue;
    private Func<float, float> customFunc = (float x) => 0f;

    private void ApproximateRange((float, float) Domain, float step)
    {
        for (float x = Domain.Item1; x < Domain.Item2; x += step)
        {
            float val = Eval(x);
            min = Mathf.Min(min, val);
            max = Mathf.Max(max, val);
        }
    }
#endregion CustomFunction

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


    private Dictionary<NoiseFunctionType, NoiseFunction> NoiseFunctions = new Dictionary<NoiseFunctionType, NoiseFunction>()
    {
        {NoiseFunctionType.Sin, new NoiseFunction(){range = (-1f, 1f), function = Mathf.Sin } },
        {NoiseFunctionType.PerlinNoise1D, new NoiseFunction(){range = (0f, 1f), function = Mathf.PerlinNoise1D } },
        {NoiseFunctionType.Sawtooth, new NoiseFunction(){range = (0f, 1f), function = (float x) => x - Mathf.Floor(x) } },
        {NoiseFunctionType.Square, new NoiseFunction(){range = (0f, 1f), function = (float x) => (x - Mathf.Floor(x)) < 0.5f ? 0f : 1f} },
        {NoiseFunctionType.Custom, new NoiseFunction(){} }
    };

    Func<float, float> func
    {
        get
        {
            return NoiseFunctions[funcType].function;
        }
    }

    public FBM1D(NoiseFunctionType funcType = NoiseFunctionType.Sin, int octaves = 4, float lacunarity = 2f, float persistence = 0.5f)
    {
        this.funcType = funcType;
        this.octaves = octaves;
        this.lacunarity = lacunarity;
        this.persistence = persistence;

        RecalculateRange();
    }

    public void SetCustomFunction(Func<float, float> customFunc)
    {
        this.customFunc = customFunc;
        NoiseFunctions[NoiseFunctionType.Custom] = new NoiseFunction() { function = customFunc };
        RecalculateRange();
    }

    private void RecalculateRange()
    {
        if (funcType == NoiseFunctionType.Custom) { ApproximateRange((-100f, 100f), 0.1f); }
        else
        {
            min = NoiseFunctions[funcType].range.Item1 * (1 - Mathf.Pow(persistence, octaves)) / (1 - persistence);
            max = NoiseFunctions[funcType].range.Item2 * (1 - Mathf.Pow(persistence, octaves)) / (1 - persistence);
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
        return (Eval(x) - min) / (max - min);
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



    #region Serialization

    public void OnAfterDeserialize()
    {
        RecalculateRange();
    }

    public void OnBeforeSerialize()
    {
        // Nothing to do here
    }

    #endregion Serialization

}