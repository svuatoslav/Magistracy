using UnityEngine;
using DATA;
using System;

public class Planet : MonoBehaviour
{
    [SerializeField] private Satellite Satellite = null;
    internal Orbit ClassOrbit { get; set; }
    private LineRenderer lineRenderer;
    internal double Eccentricity { get { return Run.Instance.e; } }
    internal double R { get { return Run.Instance.R; } }
    //internal double g { get { return _g; } }
    internal Orbits Orbit { get { return Eccentricity == 0 ? Orbits.Circular : Orbits.Elliptical; } }
    internal double StartPosition => Satellite.r_min * (Eccentricity / (1 - Eccentricity));
    internal double mu => Run.G * Run.Instance.M;
    internal double n => Math.Sqrt(mu * Math.Pow((1 - Eccentricity) / Satellite.r_min, 3));//Math.Sqrt(g*Math.Pow(R,2)/Math.Pow(_satellite.p / (1 - Math.Pow(Eccentricity, 2)), 3));
    private void Awake()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        lineRenderer.positionCount = (int)(360d / 1) + 1;
        if (Orbit == Orbits.Elliptical)
        {
            ClassOrbit = new Elliptic(Eccentricity, Satellite.r_min  / (1 - Eccentricity), Satellite.Alpha, Satellite.Beta, Satellite.r0, Satellite.Omega0,
                (new EulerAngles(Satellite.Phi0, Satellite.Psi0, Satellite.Theta0), new DimensionlessPulses(Satellite.Pphi0, Satellite.Ppsi0, Satellite.Ptheta0)),
                Run.Instance.regularPrecession);
            transform.position = new Vector3(0, 0, (float)(StartPosition / Run.Instance.Scale));
        }
        else
        {
            ClassOrbit = new Circular(Satellite.r_min, Satellite.Alpha, Satellite.Beta, Satellite.r0, Satellite.Omega0,
                (new EulerAngles(Satellite.Phi0, Satellite.Psi0, Satellite.Theta0), new DimensionlessPulses(Satellite.Pphi0, Satellite.Ppsi0, Satellite.Ptheta0)));
        }
        lineRenderer.SetPositions(ClassOrbit.DrawOrbit(1, Run.Instance.Scale));
    }
}
