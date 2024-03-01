using System.Collections;
using UnityEngine;
using DATA;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;

public class Planet : MonoBehaviour
{
    [SerializeField] private Run _Run;
    [SerializeField] private double e;
    [SerializeField] private Satellite Satellite = null;
    [SerializeField] private Orbits orbit;
    //[SerializeField] private double _g;
    internal double mu = 3.9860044e14;
    internal double G = 6720e-11;
    internal double M = 597422722e22;//59733e20;//mu/G=0,597422722e31=597422722e22
    internal double R = 6371392.896;//метров = 3959 мили//max 6377830.272=3963 мили //6371300;//6371320 6378100//transform.localScale.x / 2;
    internal Orbit ClassOrbit { get; set; }
    private LineRenderer lineRenderer;
    internal double Eccentricity { get { return e; } }
    //internal double g { get { return _g; } }
    internal Orbits Orbit { get { return orbit; } }
    internal double StartPosition => Satellite.q * (Eccentricity / (1 - Eccentricity));
    internal double n => Math.Sqrt(mu * Math.Pow((1 - Eccentricity) / Satellite.q, 3));//Math.Sqrt(g*Math.Pow(R,2)/Math.Pow(Satellite.p / (1 - Math.Pow(Eccentricity, 2)), 3));
    private void Awake()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        Run run = Camera.main.GetComponent<Run>();
        lineRenderer.positionCount = (int)(360d / 1) + 1;
        if (Orbit == Orbits.Elliptical)
        {
            ClassOrbit = new Elliptic(Eccentricity, Satellite.q  / (1 - Eccentricity), Satellite.Alpha, Satellite.r0,
                Satellite.Omega0, (new EulerAngles(Satellite.Phi0, Satellite.Psi0, Satellite.Theta0), new DimensionlessPulses(Satellite.Pphi0, Satellite.Ppsi0, Satellite.Ptheta0)), run.regularPrecession);
            transform.position = new Vector3((float)(StartPosition / _Run.Scale), 0, 0);
        }
        else
        {
            ClassOrbit = new Circular(Satellite.q, Satellite.Alpha, Satellite.r0, Satellite.Omega0, (new EulerAngles(Satellite.Phi0, Satellite.Psi0, Satellite.Theta0),
                new DimensionlessPulses(Satellite.Pphi0, Satellite.Ppsi0, Satellite.Ptheta0)));
        }
        lineRenderer.SetPositions(ClassOrbit.DrawOrbit(1, _Run.Scale));
    }
    public enum Orbits
    {
        Circular,
        Elliptical
    }
}
