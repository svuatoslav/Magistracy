using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DATA;
using Unity.VisualScripting;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;

public class Planet : MonoBehaviour
{
    [SerializeField] private double e;
    [SerializeField] private Satellite Satellite = null;
    [SerializeField] private Orbits orbit;
    [SerializeField] private double G = 9.81;
    internal Orbit ClassOrbit {  get; set; }
    private LineRenderer lineRenderer;
    internal double Eccentricity { get { return e; } }
    internal double g { get { return G; } }
    internal Orbits Orbit { get { return orbit; } }
    internal double StartPosition => (Satellite.p * Eccentricity) / (1 - Math.Pow(Eccentricity, 2));
    internal double R => transform.localScale.x / 2;
    internal double n => Math.Sqrt(g*Math.Pow(R,2)/Math.Pow(Satellite.p / (1 - Math.Pow(Eccentricity, 2)), 3));
    private void Awake()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        Run run = Camera.main.GetComponent<Run>();
        lineRenderer.positionCount = (int)(360d / 1) + 1;
        if (Orbit == Orbits.Elliptical)
        {
            ClassOrbit = new Elliptic(Eccentricity, Satellite.p / (1 - Math.Pow(Eccentricity, 2)), Satellite.Alpha, Satellite.r0,
                Satellite.Omega0, (new EulerAngles(Satellite.Phi0, Satellite.Psi0, Satellite.Theta0), new DimensionlessPulses(Satellite.Pphi0, Satellite.Ppsi0, Satellite.Ptheta0)), run.regularPrecession);
            transform.position = new Vector3((float)StartPosition, 0, 0);
        }
        else
        {
            ClassOrbit = new Circular(Satellite.StartPosition, Satellite.Alpha, Satellite.r0, Satellite.Omega0, (new EulerAngles(Satellite.Phi0, Satellite.Psi0, Satellite.Theta0),
                new DimensionlessPulses(Satellite.Pphi0, Satellite.Ppsi0, Satellite.Ptheta0)));
        }
        lineRenderer.SetPositions(ClassOrbit.DrawOrbit(1));
    }
    public enum Orbits
    {
        Circular,
        Elliptical
    }
}
