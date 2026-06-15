using System;

[Serializable]
public sealed class PlanetConfig
{
    public double Mu { get; }
    public double Radius { get; }

    public PlanetConfig(double mu, double radius)
    {
        Mu = mu;
        Radius = radius;
    }
}
