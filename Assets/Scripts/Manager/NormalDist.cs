using System;

[Serializable]
public class NormalDist
{
    private Random random = new Random();
    public double Mean;
    public double StdDev;
    public bool Clamp;
    public float NextVal()
    {
        double x1 = 1 - random.NextDouble();
        double x2 = 1 - random.NextDouble();

        double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
        if (Clamp)
            return UnityEngine.Mathf.Clamp01((float)(y1 * StdDev + Mean));
        else
            return (float)(y1 * StdDev + Mean);
    }

    public override string ToString()
    {
        return Mean.ToString();
    }
}