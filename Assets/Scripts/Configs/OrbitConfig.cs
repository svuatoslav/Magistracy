using System;

[Serializable]
public sealed class OrbitConfig
{
    public double Eccentricity { get; }

    public double r_pericenter { get; }

    public OrbitConfig(double eccentricity, double r_pericenter)
    {
        Eccentricity = eccentricity;
        this.r_pericenter = r_pericenter;
    }
}
