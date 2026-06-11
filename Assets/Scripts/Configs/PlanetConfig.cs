using System;

[Serializable]
public sealed class PlanetConfig
{
    public double Mass { get; }
    public double Radius { get; }

    public PlanetConfig(double mass, double radius)
    {
        Mass = mass;
        Radius = radius;
    }
}
