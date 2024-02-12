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
    [SerializeField] private ODEMethod methods = ODEMethod.RungeKutta_3_8;
    [SerializeField] private int timeEnd = 90;
    [SerializeField] private double MydeltaTime = 0.02;
    internal int TimeEnd {  get { return timeEnd; } }
    internal double DeltaTime { get { return MydeltaTime; } }
    internal ODEMethod odeMethod { get { return methods; } }
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
        data = new(regularPrecession);//(int)(timeEnd / deltaTime)
        solveDifferentialEquation = new(MydeltaTime);
    }
    public enum ODEMethod
    {
        RungeKutta_Claccic,
        RungeKutta_3_8,
        RungeKutta_Fehlberg45,
        RungeKutta_DormandPrince_45,
        RungeKutta_DormandPrince78,
    }
}
