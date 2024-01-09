using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using DATA;


abstract public class Orbit
{
    private protected readonly double Alpha, Beta, Omega0, r0;
    private protected readonly (EulerAngles, DimensionlessPulses) StartParam;
    internal List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions { get; private protected set; }
    public Orbit(double alpha, double r0, double omega0, (EulerAngles, DimensionlessPulses) startAngle)
    {
        Alpha = alpha;
        Beta = r0 / omega0;
        Omega0 = omega0;
        this.r0 = r0;
        StartParam = startAngle;
    }
    internal abstract double H(double nu, (EulerAngles, DimensionlessPulses) motions);
    private protected abstract double Diff_phi(double nu, (EulerAngles, DimensionlessPulses) motions);
    private protected abstract double Diff_psi(double nu, (EulerAngles, DimensionlessPulses) motions);
    private protected abstract double Diff_theta(double nu, (EulerAngles, DimensionlessPulses) motions);
    private protected abstract double Diff_pphi(double nu, (EulerAngles, DimensionlessPulses) motions);
    private protected abstract double Diff_ppsi(double nu, (EulerAngles, DimensionlessPulses) motions);
    private protected abstract double Diff_ptheta(double nu, (EulerAngles, DimensionlessPulses) motions);
    //internal abstract double Omega(double nu);
    internal abstract Vector3[] DrawOrbit(double deltaTime);
}
public class Elliptic : Orbit
{
    internal double a { get; private set; }
    internal double b { get; private set; }
    internal double e { get; private set; }
    public Elliptic(double e, double a, double alpha, double r0, double omega0, (EulerAngles, DimensionlessPulses) startParam, RegularPrecessions regularPrecession) : base(alpha, r0, omega0, startParam)
    {
        this.e = e;
        this.a = a;
        b = a * Sqrt(1 - Pow(e, 2));
        if (regularPrecession == RegularPrecessions.Cylindrical)
            ODEMotions = new() { Diff_phiCylPrec, Diff_psi, Diff_theta, Diff_pphi, Diff_ppsi, Diff_ptheta };
        else
            ODEMotions = new() { Diff_phi, Diff_psi, Diff_theta, Diff_pphi, Diff_ppsi, Diff_ptheta };
    }
    internal override double H(double nu, (EulerAngles, DimensionlessPulses) motions) => Pow(motions.Item2.ppsi, 2) / (2 * Pow(1 + e * Cos(nu), 2)
        * Pow(Sin(motions.Item1.theta), 2)) + Pow(motions.Item2.ptheta, 2) / (2 * Pow(1 + e * Cos(nu), 2)) - (Alpha * Beta * Pow(1 - Pow(e, 2), 3d / 2)
        * Cos(motions.Item1.theta) / (Pow(1 + e * Cos(nu), 2) * Pow(Sin(motions.Item1.theta), 2)) + Cos(motions.Item1.psi) / Tan(motions.Item1.theta))
        * motions.Item2.ppsi - Sin(motions.Item1.psi) * motions.Item2.ptheta + Pow(Alpha, 2) * Pow(Beta, 2) * Pow(1 - Pow(e, 2), 3)
        / (Pow(Tan(motions.Item1.theta), 2) * 2 * Pow(1 + e * Cos(nu), 2)) + Alpha * Beta * Pow(1 - Pow(e, 2), 3d / 2) * Cos(motions.Item1.psi)
        / Sin(motions.Item1.theta) + 3 * (Alpha - 1) * (1 + e * Cos(nu)) * Pow(Cos(motions.Item1.theta), 2) / 2;
    private protected override double Diff_phi(double nu, (EulerAngles, DimensionlessPulses) motions) => r0 + Omega(nu) * Cos(motions.Item1.psi) * Sin(motions.Item1.theta) - 
        Diff_psi(nu, motions) * Cos(motions.Item1.theta);
    private protected double Diff_phiCylPrec(double nu, (EulerAngles, DimensionlessPulses) motions) => Omega0 * (Beta - Pow(1 + e * Cos(nu), 2) / Pow(1 - Pow(e, 2), 3d / 2));
    private protected override double Diff_psi(double nu, (EulerAngles, DimensionlessPulses) motions) => ((motions.Item2.ppsi - Alpha * Beta * Pow(1 - Pow(e, 2), 3d / 2)
        * Cos(motions.Item1.theta)) / (Pow(1 + e * Cos(nu), 2) * Sin(motions.Item1.theta)) - Cos(motions.Item1.psi) * Cos(motions.Item1.theta)) / Sin(motions.Item1.theta);
    private protected override double Diff_theta(double nu, (EulerAngles, DimensionlessPulses) motions) => motions.Item2.ptheta / Pow(1 + e * Cos(nu), 2) - Sin(motions.Item1.psi);
    private protected override double Diff_pphi(double nu, (EulerAngles, DimensionlessPulses) motions) => 0;
    private protected override double Diff_ppsi(double nu, (EulerAngles, DimensionlessPulses) motions) => Sin(motions.Item1.psi) * (Alpha * Beta * Pow(1 - Pow(e, 2), 3d / 2)
        - Cos(motions.Item1.theta) * motions.Item2.ppsi) / Sin(motions.Item1.theta) + Cos(motions.Item1.psi) * motions.Item2.ptheta;
    private protected override double Diff_ptheta(double nu, (EulerAngles, DimensionlessPulses) motions) => Cos(motions.Item1.theta) * (((motions.Item2.ppsi - 
        Alpha * Beta * Pow(1 - Pow(e, 2), 3d / 2) * Cos(motions.Item1.theta)) * ((motions.Item2.ppsi - Alpha * Beta * Pow(1 - Pow(e, 2), 3d / 2) * 
        Cos(motions.Item1.theta)) / Pow(Sin(motions.Item1.theta), 2) - Alpha * Beta * Pow(1 - Pow(e, 2), 3d / 2) / Cos(motions.Item1.theta)) / Pow(1 + e * Cos(nu), 2) + 
        Cos(motions.Item1.psi) * (Alpha * Beta * Pow(1 - Pow(e, 2), 3d / 2) - Cos(motions.Item1.theta) * motions.Item2.ppsi) / Sin(motions.Item1.theta)) 
        / Sin(motions.Item1.theta) + 3 * (Alpha - 1) * (1 + e * Cos(nu)) * Sin(motions.Item1.theta)) - Cos(motions.Item1.psi) * motions.Item2.ppsi;
    private protected double Omega(double nu) => Omega0 * Pow(1 + e * Cos(nu), 2) / Pow(1 - Pow(e, 2), 3d / 2);

    internal override Vector3[] DrawOrbit(double step)
    {
        Vector3[] positions = new Vector3[(int)(360 / step) + 1];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = new Vector3((float)(a * Cos((i * step) * Mathf.Deg2Rad)), (float)(b * Sin((i * step) * Mathf.Deg2Rad)), 0);
        }
        return positions;
    }
}
public class Circular : Orbit
{
    internal double R { get; private set; }
    public Circular(double R, double alpha, double r0, double omega0, (EulerAngles, DimensionlessPulses) startParam) : base(alpha, r0, omega0, startParam) 
    { 
        this.R = R;
        ODEMotions = new() { Diff_phi, Diff_psi, Diff_theta, Diff_pphi, Diff_ppsi, Diff_ptheta };
    }

    internal override double H(double nu, (EulerAngles, DimensionlessPulses) motions) => (Pow(motions.Item2.ppsi / Sin(motions.Item1.theta), 2)
        + Pow(motions.Item2.ptheta, 2)) / 2 - (Alpha * Beta / Sin(motions.Item1.theta) + Cos(motions.Item1.psi)) * motions.Item2.ppsi / Tan(motions.Item1.theta)
        - Sin(motions.Item1.psi) * motions.Item2.ptheta + Pow(Alpha * Beta / Tan(motions.Item1.theta), 2) / 2 + Alpha * Beta
        * Cos(motions.Item1.psi) / Sin(motions.Item1.theta) + (3 * (Alpha - 1)) * Pow(Cos(motions.Item1.theta), 2) / 2;
    private protected override double Diff_phi(double nu, (EulerAngles, DimensionlessPulses) motions) => Omega0 * (Beta + Cos(StartParam.Item1.psi) * Sin(StartParam.Item1.theta));
    private protected override double Diff_psi(double nu, (EulerAngles, DimensionlessPulses) motions) => (motions.Item2.ppsi - Alpha * Beta * Cos(motions.Item1.theta)) / Pow(Sin(motions.Item1.theta), 2)
        - Cos(motions.Item1.psi) / Tan(motions.Item1.theta);
    private protected override double Diff_theta(double nu, (EulerAngles, DimensionlessPulses) motions) => motions.Item2.ptheta - Sin(motions.Item1.psi);
    private protected override double Diff_pphi(double nu, (EulerAngles, DimensionlessPulses) motions) => 0;

    private protected override double Diff_ppsi(double nu, (EulerAngles, DimensionlessPulses) motions) => Sin(motions.Item1.psi) * (Alpha * Beta - Cos(motions.Item1.theta) * motions.Item2.ppsi)
        / Sin(motions.Item1.theta) + Cos(motions.Item1.psi) * motions.Item2.ptheta;
    private protected override double Diff_ptheta(double nu, (EulerAngles, DimensionlessPulses) motions) => ((Pow(Alpha * Beta * Cos(motions.Item1.theta) - motions.Item2.ppsi, 2) / Sin(motions.Item1.theta)
        + Cos(motions.Item1.psi) * (Alpha * Beta - motions.Item2.ppsi * Cos(motions.Item1.theta))) / Tan(motions.Item1.theta) + Alpha * Beta * (Alpha * Beta * Cos(motions.Item1.theta)
        - motions.Item2.ppsi)) / Sin(motions.Item1.theta) + ((3 * (Alpha - 1)) * Sin(2 * motions.Item1.theta)) / 2 - motions.Item2.ppsi * Cos(motions.Item1.psi);
    internal override Vector3[] DrawOrbit(double step)
    {
        Vector3[] positions = new Vector3[(int)(360 / step) + 1];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = new Vector3((float)(R * Cos((i * step) * Mathf.Deg2Rad)), (float)(R * Sin((i * step) * Mathf.Deg2Rad)), 0);
        }
        return positions;
    }
}
