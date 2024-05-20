using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DATA;
using System;

public class Satellite : MonoBehaviour
{
    [Header("ѕросо так")]
    //[SerializeField] private Run Run;
    [SerializeField] private Planet _Planet = null;
    [SerializeField] private UISinulation _UISinulation = null;
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
    internal int TimeIndex = 0;
    internal int _speedTime = 1;
    internal double AB { get { return Run.Instance.AB; } }
    internal double C { get { return Run.Instance.C; } }
    //internal double V0 { get { return _V0; } }
    internal double r_min { get { return Run.Instance.R + Run.Instance.Height; } }
    internal double Phi0 { get { return Run.Instance.Phi0; } }
    internal double r0
    {
        get
        {
            if (Run.Instance.SelectingParameter == 0)
                return Run.Instance.Beta * Omega0;
            else if (Run.Instance.SelectingParameter == 1)
                return Run.Instance.r0;
            else
                return Omega0 * (((Math.Sin(Theta0) * Math.Cos(Psi0) * Math.Cos(Theta0) * Math.Pow(1 + _Planet.Eccentricity, 2) - Ppsi0) * Math.Cos(Theta0) 
                    - Run.Instance.PhiDot0 * Math.Pow(Math.Sin(Theta0), 2) * Math.Pow(1 + _Planet.Eccentricity, 2)) 
                    * (_Planet.Eccentricity - 1) * Math.Sqrt(1 - Math.Pow(_Planet.Eccentricity, 2))
                    - Math.Cos(Psi0) * Omega0 * Math.Pow(1 + _Planet.Eccentricity, 3) * Math.Pow(Math.Sin(Theta0), 3))
                    / (Math.Pow(1 + _Planet.Eccentricity, 2) * (1 - _Planet.Eccentricity) * (Omega0 * Math.Pow(Math.Sin(Theta0), 2)
                    * Math.Sqrt(1 - Math.Pow(_Planet.Eccentricity, 2)) + Alpha * Math.Pow(Math.Cos(Theta0), 2) * Math.Pow(1 - _Planet.Eccentricity, 2)));
        }
    }
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
    internal double p => r_min * (1 + _Planet.Eccentricity);
    internal double SectorSpeed => Math.Sqrt(_Planet.mu * p);//(StartPosition * V0) / 2;
    internal double Omega0 => Math.Sqrt(_Planet.mu * Math.Pow((1 - _Planet.Eccentricity) / r_min, 3));
    internal double Alpha => Run.Instance.C / Run.Instance.AB;
    internal double Beta
    {
        get
        {
            if (Run.Instance.SelectingParameter == 0)
                return Run.Instance.Beta;
            else if (Run.Instance.SelectingParameter == 1)
                return Run.Instance.r0 / Omega0;
            else
                return r0 / Omega0;
        }
    }
    internal Vector3 StartPosition => new(0, 0, (float)((_Planet.StartPosition + r_min) / Run.Instance.Scale));

    private void Start()
    {
        transform.position = StartPosition;
        ///
        /// вычислени€
        ///
        Run.Instance.data.FlightTime = new double[(int)Math.Round(Run.Instance.TimeEnd / 0.02, 0) + 1];
        for (int i = 1; i < Run.Instance.data.FlightTime.Length; i++)
            Run.Instance.data.FlightTime[i] = Math.Round(Run.Instance.data.FlightTime[i - 1] + 0.02, 2);

        //Run.Instance.data.E = new double[FlightTime.Length];
        Run.Instance.data.Nu = new double[Run.Instance.data.FlightTime.Length];
        Run.Instance.data.NuAbs = new double[Run.Instance.data.FlightTime.Length];
        Run.Instance.data.H = new double[Run.Instance.data.FlightTime.Length];

        SolveKeplerEquation solveKeplerEquation;
        if (Run.Instance.waysSolveKeplerEquation == WaysSolveKeplerEquation.Iteration_method)
            solveKeplerEquation = new IterationMethod(_Planet.n, _Planet.Eccentricity);
        else if (Run.Instance.waysSolveKeplerEquation == WaysSolveKeplerEquation.DecompositionEccentricity)
            solveKeplerEquation = new DecompositionDegreesEccentricity(_Planet.n, _Planet.Eccentricity);
        else
            solveKeplerEquation = new DenbyMethod(_Planet.n, _Planet.Eccentricity);
        Run.Instance.data.E = solveKeplerEquation.Calculate(Run.Instance.ApproximationNumberKeplerEquation, Run.Instance.data.FlightTime);

        Angle = new EulerAngles[Run.Instance.data.FlightTime.Length];
        double nu_min = double.MaxValue;
        int i_min = 0;
        double nu_max = 0;
        int i_max = 0;
        double period = 0;
        Run.Instance.data.Nu[0] = Elliptic.EtoNu(Run.Instance.data.E[0], _Planet.Eccentricity);
        for (int i = 1; i < Run.Instance.data.FlightTime.Length; i++)
        {
            Run.Instance.data.Nu[i] = Elliptic.EtoNu(Run.Instance.data.E[i], _Planet.Eccentricity);//Debug.LogWarning($"¬рем€ {FlightTime[i]} -> E {E[i] * Mathf.Rad2Deg} -> nu {Run.Instance.data.NuTemp[i] * Mathf.Rad2Deg}");
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
            if (Run.Instance.data.NuAbs[i] - Run.Instance.data.NuAbs[i - 1] < nu_min)
            {
                i_min = i;
                nu_min = Run.Instance.data.NuAbs[i] - Run.Instance.data.NuAbs[i - 1];
            }
            if (Run.Instance.data.NuAbs[i] - Run.Instance.data.NuAbs[i - 1] > nu_max)
            {
                i_max = i;
                nu_max = Run.Instance.data.NuAbs[i] - Run.Instance.data.NuAbs[i - 1];
            }
        }
        Debug.LogWarning($"минимальна€ разница {nu_min}, в градусах {nu_min * Mathf.Rad2Deg}, в {i_min} элементе");
        Debug.LogWarning($"максимальна€ разница {nu_max}, в градусах {nu_max * Mathf.Rad2Deg}, в {i_max} элементе");
        if (Run.Instance.odeMethod == ODEMethod.RungeKutta_Claccic || Run.Instance.odeMethod == ODEMethod.RungeKutta_3_8)
            Run.Instance.data.MotionsAngle = SolveDifferentialEquation.RKCalculate((new EulerAngles(Phi0, Psi0, Theta0), new DimensionlessPulses(Pphi0, Ppsi0, Ptheta0)), Run.Instance.data.NuAbs, _Planet.ClassOrbit.ODEMotions, Run.Instance.odeMethod, Run.Instance.StepIntegration);
        else
            Run.Instance.data.MotionsAngle = SolveDifferentialEquation.RKCalculate((new EulerAngles(Phi0, Psi0, Theta0), new DimensionlessPulses(Pphi0, Ppsi0, Ptheta0)), Run.Instance.data.NuAbs, _Planet.ClassOrbit.ODEMotions, Run.Instance.odeMethod, Run.Instance.StepIntegration, Run.Instance.Epsilon);
        for (int i = 0; i < Run.Instance.data.FlightTime.Length; i++)
        {
            Run.Instance.data.H[i] = _Planet.ClassOrbit.H(Run.Instance.data.NuAbs[i], Run.Instance.data.MotionsAngle[i]);
            Angle[i] = (EulerAngles.ToDegrees(EulerAngles.ToUnityAngle(Run.Instance.data.MotionsAngle[i].Item1)));
            //Angle[i] = (EulerAngles.ToDegrees(EulerAngles.ToUnityAngle(Run.Instance.data.MotionsAngle[i].Item1)));
        }
        Run.Instance.save.SaveGame(Run.Instance.data, Run.Instance.odeMethod, Run.Instance.waysSolveKeplerEquation);
        gameObject.transform.rotation = Quaternion.Euler(new Vector3((float)Angle[0].phi, (float)Angle[0].psi, (float)Angle[0].theta));
        //gameObject.transform.rotation = Quaternion.Euler(new Vector3((float)Angle[0].psi, (float)Angle[0].phi, (float)Angle[0].theta));//цилиндр
        //gameObject.transform.rotation = Quaternion.Euler(new Vector3((float)Angle[0].psi, (float)Angle[0].phi, (float)Angle[0].theta));//цилиндр
    }
    private void FixedUpdate()
    {
        if (TimeIndex < Run.Instance.data.Nu.Length)
        {
            //Debug.Log($"индекс {TimeIndex}, угол {Run.Instance.data.Nu[TimeIndex]}");
            transform.position = LinearMotion();
            gameObject.transform.rotation = AttitudeMotion();
            _UISinulation.ChangeTime(TimeIndex);
            if (TimeIndex + _speedTime < Angle.Length)
                TimeIndex += _speedTime;
            else if (TimeIndex < Angle.Length - 1)
                TimeIndex++;
        }
    }
    internal double r(double nu) => p / (1 + _Planet.Eccentricity * Math.Cos(nu));
    private Vector3 LinearMotion() => _Planet.transform.position + new Vector3(-(float)(r(Run.Instance.data.Nu[TimeIndex]) * Math.Sin(Run.Instance.data.Nu[TimeIndex]) / Run.Instance.Scale), 0, (float)(r(Run.Instance.data.Nu[TimeIndex]) * Math.Cos(Run.Instance.data.Nu[TimeIndex]) / Run.Instance.Scale));
    //цилиндр
    //private Quaternion AttitudeMotion() => Quaternion.Euler(new Vector3((float)Angle[TimeIndex].psi, (float)(Angle[TimeIndex].phi - Run.Instance.data.Nu[TimeIndex] * Mathf.Rad2Deg), (float)Angle[TimeIndex].theta));//-(float)Run.Instance.data.Nu[TimeIndex] * Mathf.Rad2Deg
    private Quaternion AttitudeMotion() => Quaternion.Euler(new Vector3((float)Angle[TimeIndex].phi, (float)(Angle[TimeIndex].psi - Run.Instance.data.Nu[TimeIndex] * Mathf.Rad2Deg), (float)Angle[TimeIndex].theta));//-(float)Run.Instance.data.Nu[TimeIndex] * Mathf.Rad2Deg
}
