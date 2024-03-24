using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DATA;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Windows;
using System.Linq;
using Unity.VisualScripting;

public class Satellite : MonoBehaviour
{
    [Header("Просо так")]
    //[SerializeField] private Run Run;
    [SerializeField] private Planet _Planet = null;
    //[SerializeField] private double _AB;
    //[SerializeField] private double _C;
    //[SerializeField] private double phi0;
    //[SerializeField] private double psi0;
    //[SerializeField] private double theta0;
    //[SerializeField] private double pphi0;
    //[SerializeField] private double ppsi0;
    //[SerializeField] private double ptheta0;
    ////[SerializeField] private double _V0;
    //[SerializeField] private double _r_min;
    //[SerializeField] private double _r0;
    internal EulerAngles[] Angle;
    private int _index = 0;
    private int _speedTime = 1;
    internal double AB { get { return Run.Instance.AB; } }
    internal double C { get { return Run.Instance.C; } }
    //internal double V0 { get { return _V0; } }
    internal double q { get { return Run.Instance.R + Run.Instance.Height; } }
    internal double Phi0 { get { return Run.Instance.Phi0; } }
    internal double r0 { get { return Run.Instance.r0; } }
    internal double Psi0
    {
        get
        {
            if (Run.Instance.regularPrecession == RegularPrecessions.Cylindrical)
                return Math.PI;
            else if (Run.Instance.regularPrecession == RegularPrecessions.Hyperboloidal)
                return Math.Acos(-Alpha * Beta);
            else if (Run.Instance.regularPrecession == RegularPrecessions.Conical)
                return 0;
            else
                return Run.Instance.Psi0;
        }
    }
    internal double Theta0
    {
        get
        {
            if (Run.Instance.regularPrecession == RegularPrecessions.Cylindrical)
                return Math.PI / 2;
            else if (Run.Instance.regularPrecession == RegularPrecessions.Hyperboloidal)
                return Math.PI / 2;
            else if (Run.Instance.regularPrecession == RegularPrecessions.Conical)
                return Math.Asin(Alpha * Beta / (3 * Alpha - 4));
            else
                return Run.Instance.Theta0;
        }
    }
    internal double Pphi0 => Alpha * Beta * Math.Pow((1 - Math.Pow(_Planet.Eccentricity, 2)), 3d / 2);
    internal double Ppsi0
    {
        get
        {
            if (Run.Instance.regularPrecession == RegularPrecessions.Cylindrical || Run.Instance.regularPrecession == RegularPrecessions.Hyperboloidal)
                return 0;
            else if (Run.Instance.regularPrecession == RegularPrecessions.Conical)
                return 3 * (Alpha - 1) * (Alpha * Beta / (3 * Alpha - 4)) * Math.Cos(Theta0);
            else
                return Run.Instance.Ppsi0;
        }
    }
    internal double Ptheta0 
    {
        get
        {
            if (Run.Instance.regularPrecession == RegularPrecessions.Cylindrical || Run.Instance.regularPrecession == RegularPrecessions.Conical)
                return 0;
            else if (Run.Instance.regularPrecession == RegularPrecessions.Hyperboloidal)
                return Math.Sin(Psi0);
            else
                return Run.Instance.Ptheta0;
        }
    }
    //internal double StartPosition
    //{
    //    get
    //    {
    //        if (_Planet.Orbit == Planet.Orbits.Elliptical)
    //            return (_Planet.g * Math.Pow(_Planet.R, 2) * (1 + _Planet.Eccentricity)) / Math.Pow(V0, 2);
    //        else
    //            return (_Planet.g * Math.Pow(_Planet.R, 2)) / Math.Pow(V0, 2);
    //    }
    //}
    internal double p => q * (1 + _Planet.Eccentricity);
    internal double SectorSpeed => Math.Sqrt(_Planet.mu * p);//(StartPosition * V0) / 2;
    internal double Omega0 => 2 * Math.PI * Math.Sqrt(_Planet.mu * Math.Pow((1 - _Planet.Eccentricity) / q, 3));
    internal double Alpha => Run.Instance.C / Run.Instance.AB;
    internal double Beta => Run.Instance.r0 / Omega0;

    private void Start()
    {
        transform.position = new Vector3((float)((_Planet.StartPosition + q) / Run.Instance.Scale), 0, 0);
        ///
        /// вычисления
        ///
        double[] flightTime = new double[(int)Math.Round(Run.Instance.TimeEnd / 0.02, 0) + 1];
        for (int i = 1; i < flightTime.Length; i++)
            flightTime[i] = Math.Round(flightTime[i - 1] + 0.02,2);

        SolveKeplerEquation solveKeplerEquation;
        if (Run.Instance.waysSolveKeplerEquation == WaysSolveKeplerEquation.ClassicApproximation)
            solveKeplerEquation = new ClassicMethodSuccessiveApproximations(_Planet.n, _Planet.Eccentricity);
        else if (Run.Instance.waysSolveKeplerEquation == WaysSolveKeplerEquation.DecompositionEccentricity)
            solveKeplerEquation = new DecompositionDegreesEccentricity(_Planet.n, _Planet.Eccentricity);
        else
            solveKeplerEquation = new DenbyMethod(_Planet.n, _Planet.Eccentricity);
        double[] E = solveKeplerEquation.Calculate(Run.Instance.ApproximationNumberKeplerEquation, flightTime);

        Run.Instance.data.Nu = new double[E.Length];
        Run.Instance.data.NuAbs = new double[E.Length];
        Run.Instance.data.H = new double[E.Length];
        Angle = new EulerAngles[E.Length];
        double period = 0;
        Run.Instance.data.Nu[0] = Elliptic.EtoNu(E[0], _Planet.Eccentricity);
        for (int i = 1; i < E.Length; i++)
        {
            Run.Instance.data.Nu[i] = Elliptic.EtoNu(E[i], _Planet.Eccentricity);//Debug.LogWarning($"Время {flightTime[i]} -> E {E[i] * Mathf.Rad2Deg} -> nu {Run.Instance.data.NuTemp[i] * Mathf.Rad2Deg}");
            if (Run.Instance.data.Nu[i] < 0)
            {
                if (Run.Instance.data.Nu[i - 1] > 0)
                    period += Math.PI;
                Run.Instance.data.NuAbs[i] = Math.PI + Run.Instance.data.Nu[i] + period;
            }
            else
            {
                if (Run.Instance.data.Nu[i - 1] < 0)
                    period += Math.PI;
                Run.Instance.data.NuAbs[i] = Run.Instance.data.Nu[i] + period;
            }
        }
        if (Run.Instance.odeMethod == ODEMethod.RungeKutta_Claccic || Run.Instance.odeMethod == ODEMethod.RungeKutta_3_8)
            Run.Instance.data.MotionsAngle = SolveDifferentialEquation.RKCalculate((new EulerAngles(Phi0, Psi0, Theta0), new DimensionlessPulses(Pphi0, Ppsi0, Ptheta0)), Run.Instance.data.NuAbs, _Planet.ClassOrbit.ODEMotions, Run.Instance.odeMethod, Run.Instance.StepIntegration);
        else
            Run.Instance.data.MotionsAngle = SolveDifferentialEquation.RKCalculate((new EulerAngles(Phi0, Psi0, Theta0), new DimensionlessPulses(Pphi0, Ppsi0, Ptheta0)), Run.Instance.data.NuAbs, _Planet.ClassOrbit.ODEMotions, Run.Instance.odeMethod, Run.Instance.StepIntegration, Run.Instance.Epsilon);
        ;
        for (int i = 0; i < E.Length; i++)
        {
            _Planet.ClassOrbit.H(Run.Instance.data.NuAbs[i], Run.Instance.data.MotionsAngle[i]);
            Angle[i] = (EulerAngles.ToDegrees(EulerAngles.ToUnityAngle(Run.Instance.data.MotionsAngle[i].Item1)));
        }
        //for (int NumberApproximation = 0, j = stepFixedTime; NumberApproximation < data.Angle.Count; NumberApproximation++)
        //{
        //    if (j == stepFixedTime)
        //    {
        //        Angle.Add(data.Angle[NumberApproximation]);
        //        j = 1;
        //        //Debug.Log($"timeEnd: {Time0} {Angle.Last()}");
        //    }
        //    else
        //        j++;
        //    Time0 += deltaTime;
        //}
        //Run.save.SaveGame(Run.data, Run.odeMethod, 0.02, (int)(0.02 / Run.DeltaTime));
        //Math.Round(0.019,);
        
    }
    private void FixedUpdate()
    {
        if (_index < Run.Instance.data.Nu.Length)
        {
            LinearMotion();
            AttitudeMotion();
            if (_index + _speedTime <= Angle.Length)
                _index += _speedTime;
            else
                _index++;
            Debug.Log($"время {Time.time}, индекс {_index}, угол {Run.Instance.data.Nu[_index]}");
        }
    }
    internal double r(double nu) => p / (1 + _Planet.Eccentricity * Math.Cos(nu));
    private void LinearMotion()
    {
        transform.position = _Planet.transform.position + new Vector3((float)(r(Run.Instance.data.Nu[_index]) * Math.Cos(Run.Instance.data.Nu[_index]) / Run.Instance.Scale), (float)(r(Run.Instance.data.Nu[_index]) * Math.Sin(Run.Instance.data.Nu[_index]) / Run.Instance.Scale), 0);
    }
    private void AttitudeMotion()
    {
        gameObject.transform.rotation = Quaternion.Euler(new Vector3((float)Angle[_index].phi, (float)Angle[_index].psi, (float)Angle[_index].theta));
    }
}
