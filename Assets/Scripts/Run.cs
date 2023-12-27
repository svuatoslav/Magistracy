using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System;
using DATA;

public class Run : MonoBehaviour
{
    [SerializeField] private GameObject Planet = null;
    [SerializeField] private GameObject Satellite = null;
    [SerializeField] private RegularPrecessions _RegularPrecession;
    [SerializeField] private ODEMethod methods = ODEMethod.RungeKutta3to8;
    [SerializeField] private int nu = 90;
    [SerializeField] private double MydeltaTime = 0.02;
    internal int Nu {  get { return nu; } }
    internal double DeltaTime { get { return MydeltaTime; } }
    internal RegularPrecessions regularPrecession { get { return _RegularPrecession; } }

    internal Save save;
    internal Data data;
    internal SolveDifferentialEquation solveDifferentialEquation;
    private void Awake()
    {
        var planet = Planet.GetComponent<Planet>();
        var satellite = Satellite.GetComponent<Satellite>();
        Satellite.GetComponent<Satellite>();
        save = new Save();
        data = new(regularPrecession);//(int)(nu / deltaTime)
        solveDifferentialEquation = new(MydeltaTime);
    }
    public enum ODEMethod
    {
        RungeKuttaClaccic,
        RungeKutta3to8,
        RungeKuttaFehlberg45,
        RungeKuttaDormandPrince45,
        //RungeKuttaDormandPrince78,
    }
}
