using System;

[Serializable]
public sealed class SatelliteConfig
{
    public double AB { get; }
    public double C { get; }
    public double r0 { get; }

    public double Phi { get; }
    public double Psi { get; }
    public double Theta { get; }
    public double Pphi { get; }
    public double Ppsi { get; }
    public double Ptheta { get; }

    public SatelliteConfig(
        double phi, double psi, double theta,
        double pphi, double ppsi, double ptheta,
        double r0, double ab, double c)
    {
        Phi = phi;
        Psi = psi;
        Theta = theta;
        Pphi = pphi;
        Ppsi = ppsi;
        Ptheta = ptheta;
        this.r0 = r0;
        AB = ab;
        C = c;
    }
}
