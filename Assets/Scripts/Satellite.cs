using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DATA;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Windows;
using System.Linq;

public class Satellite : MonoBehaviour
{
    [SerializeField] private Run _Run;
    [SerializeField] private Planet _Planet = null;
    [SerializeField] private double _A;
    [SerializeField] private double _B;
    [SerializeField] private double _C;
    [SerializeField] private double phi0;
    [SerializeField] private double psi0;
    [SerializeField] private double theta0;
    [SerializeField] private double pphi0;
    [SerializeField] private double ppsi0;
    [SerializeField] private double ptheta0;
    [SerializeField] private double _V0;
    [SerializeField] private double _r0;
    internal List<EulerAngles> Angle = new();
    internal double A { get { return _A; } }
    internal double B { get { return _B; } }
    internal double C { get { return _C; } }
    internal double V0 { get { return _V0; } }
    internal double Phi0 { get { return phi0; } }
    internal double r0 { get { return _r0; } }
    internal double Psi0
    {
        get
        {
            if (_Run.regularPrecession == RegularPrecessions.Cylindrical)
                return Math.PI;
            if (_Planet.Orbit == Planet.Orbits.Elliptical || _Run.regularPrecession == RegularPrecessions.MyParametrs)
                return psi0;
            else
            {
                if (_Run.regularPrecession == RegularPrecessions.Hyperboloidal)
                    return Math.Acos(-Alpha * Beta);
                else
                    return 0;
            }
        }
    }
    internal double Theta0
    {
        get
        {
            if (_Run.regularPrecession == RegularPrecessions.Cylindrical)
                return Math.PI / 2;
            if (_Planet.Orbit == Planet.Orbits.Elliptical || _Run.regularPrecession == RegularPrecessions.MyParametrs)
                return theta0;
            else
            {
                if (_Run.regularPrecession == RegularPrecessions.Hyperboloidal)
                    return Math.PI / 2;
                else
                    return Math.Asin(Alpha * Beta / (3 * Alpha - 4));
            }
        }
    }
    internal double Pphi0
    {
        get
        {
            if (_Planet.Orbit == Planet.Orbits.Elliptical)
                return Alpha * Beta * Math.Pow((1 - Math.Pow(_Planet.Eccentricity, 2)), 3d / 2);// Math.Pow((1 - _Planet.Eccentricity) / (1 + _Planet.Eccentricity), 3d / 2);
            else
                return Alpha * Beta;
        }
    }
    internal double Ppsi0
    {
        get
        {
            if (_Run.regularPrecession == RegularPrecessions.Cylindrical)
                return 0;
            if (_Planet.Orbit == Planet.Orbits.Elliptical || _Run.regularPrecession == RegularPrecessions.MyParametrs)
                return ppsi0;
            else
            {
                if (_Run.regularPrecession == RegularPrecessions.Hyperboloidal)
                    return 0;
                else
                    return 3 * (Alpha - 1) * Alpha * Beta / (3 * Alpha - 4) * Math.Cos(Theta0);// Math.Sin(Theta0)
            }
        }
    }
    internal double Ptheta0 
    {
        get
        {
            if (_Run.regularPrecession == RegularPrecessions.Cylindrical)
                return 0;
            if (_Planet.Orbit == Planet.Orbits.Elliptical || _Run.regularPrecession == RegularPrecessions.MyParametrs)
                return ptheta0;
            else
            {
                if (_Run.regularPrecession == RegularPrecessions.Hyperboloidal)
                    return Math.Sin(Psi0);
                else
                    return 0;
            }
        }
    }
    internal double StartPosition
    {
        get
        {
            if (_Planet.Orbit == Planet.Orbits.Elliptical)
                return (_Planet.g * Math.Pow(_Planet.R, 2) * (1 + _Planet.Eccentricity)) / Math.Pow(V0, 2);
            else
                return (_Planet.g * Math.Pow(_Planet.R, 2)) / Math.Pow(V0, 2);
        }
    }
    internal double p
    {
        get
        {
            if (_Planet.Orbit == Planet.Orbits.Elliptical)
                return StartPosition * (1 + _Planet.Eccentricity);
            else
                return StartPosition;
        }
    }
    internal double SectorSpeed => (StartPosition * V0) / 2;
    internal double r 
    { 
        get
        {
            if (_Planet.Orbit == Planet.Orbits.Elliptical)
                return p / (1 + _Planet.Eccentricity * Math.Cos(Time.time * Mathf.Deg2Rad));
            else
                return p;
        } 
    }
    internal double Omega0
    {
        get
        {
            if (_Planet.Orbit == Planet.Orbits.Elliptical)
                return Math.Pow(V0, 3) / (_Planet.g * Math.Pow(_Planet.R, 2)) * Math.Pow((1 - _Planet.Eccentricity) / (1 + _Planet.Eccentricity), 3d / 2);
            else
                return Math.Pow(V0, 3) / (_Planet.g * Math.Pow(_Planet.R, 2));
        }
    }
    internal double Alpha => C / A;
    internal double Beta => r0 / Omega0;

    private double Time0 = 0;
    private int k = 0;
    private (EulerAngles, DimensionlessPulses) motion;
    private void Start()
    {
        if (0 >= Alpha || Alpha > 2)
        {
            Debug.LogError("alpha error");
            EditorApplication.isPaused = true;
        }
        if (_Run.regularPrecession == RegularPrecessions.Hyperboloidal && Math.Abs(Alpha * Beta) > 1 && _Planet.Orbit == Planet.Orbits.Circular)
        {
            Debug.LogError($"|Alpha * Beta| > 1 <=> {Math.Abs(Alpha * Beta)} > 1");
            EditorApplication.isPaused = true;
        }
        if (_Run.regularPrecession == RegularPrecessions.Conical && (Math.Abs(Alpha * Beta) > Math.Abs(3 * Alpha - 4) || Alpha == 4d / 3) && _Planet.Orbit == Planet.Orbits.Circular)
        {
            Debug.LogError($"|Alpha * Beta| > |3 * Alpha - 4| <=> {Math.Abs(Alpha * Beta)} > {Math.Abs(3 * Alpha - 4)} или Alpha = 4 / 3 <=> {Alpha} = {4d / 3}");
            EditorApplication.isPaused = true;
        }
        if (V0 >= Math.Sqrt(_Planet.g * _Planet.R * (1 + _Planet.Eccentricity)))
        {
            Debug.LogError("V0 error");
            EditorApplication.isPaused = true;
        }
        Debug.Log($"V0 = {V0} < {Math.Sqrt(_Planet.g * _Planet.R * (1 + _Planet.Eccentricity))}");
        Debug.Log($"Omega0 = {Omega0}");
        transform.position = new Vector3((float)_Planet.StartPosition, 0, 0) + new Vector3((float)StartPosition, 0, 0);
        ///
        /// вычисления
        ///
        double[] flightTime = new double[(int)Math.Round(_Run.TimeEnd / 0.02, 0) + 1];
        for (int i = 1; i < flightTime.Length; i++)
            flightTime[i] = Math.Round(flightTime[i - 1] + 0.02,2);
        SolveKeplerEquation solveKeplerEquation = new DecompositionDegreesEccentricity(_Planet.n, _Planet.Eccentricity);
        double[] E = solveKeplerEquation.Calculate(3, flightTime);
        double[] nu = new double[E.Length];
        for (int i = 0; i < E.Length; i++)
        {
            nu[i] = Elliptic.EtoNu(E[i], _Planet.Eccentricity);
            Debug.LogWarning($"Время {flightTime[i]} -> E {E[i]} -> nu {nu[i]}");
        }
        /*
        _Run.data.MotionsAngle.Add((new EulerAngles(Phi0, Psi0, Theta0), new DimensionlessPulses(Pphi0, Ppsi0, Ptheta0)));
        _Run.data.H.Add(_Planet.ClassOrbit.H(0, _Run.data.MotionsAngle[^1]));
        (EulerAngles, DimensionlessPulses) temp = _Run.data.MotionsAngle.Last();
        
        for (int i = 1; i < flightTime.Length; i++)
        {
            //if (_Run.odeMethod == Run.ODEMethod.RungeKutta_Claccic)
            //    temp = _Run.solveDifferentialEquation.RKClassic(temp, _Planet.ClassOrbit.ODEMotions, Time0, );
            //else if (_Run.odeMethod == Run.ODEMethod.RungeKutta_3_8)
            //    temp = _Run.solveDifferentialEquation.RK3to8(temp, _Planet.ClassOrbit.ODEMotions, Time0);
            //else if (_Run.odeMethod == Run.ODEMethod.RungeKutta_Fehlberg45)
            //    temp = _Run.solveDifferentialEquation.RKFehlberg(temp, _Planet.ClassOrbit.ODEMotions, Time0);

            if (_Run.odeMethod == Run.ODEMethod.RungeKutta_Claccic)
                motion = _Run.solveDifferentialEquation.RKClassic(_Run.data.MotionsAngle[^1], _Planet.ClassOrbit.ODEMotions, Time0);
            else if (_Run.odeMethod == Run.ODEMethod.RungeKutta_3_8)
                motion = _Run.solveDifferentialEquation.RK3to8(_Run.data.MotionsAngle[^1], _Planet.ClassOrbit.ODEMotions, Time0);
            else if (_Run.odeMethod == Run.ODEMethod.RungeKutta_Fehlberg45)
                motion = _Run.solveDifferentialEquation.RKFehlberg(_Run.data.MotionsAngle[^1], _Planet.ClassOrbit.ODEMotions, Time0);
            Time0 += _Run.DeltaTime;
            _Run.data.MotionsAngle.Add(motion);
            _Run.data.H.Add(_Planet.ClassOrbit.H(0, _Run.data.MotionsAngle[^1]));
        }
        Time0 = 0;
        //foreach (var motion in _Run.data.MotionsAngle)
        //    Angle.Add(EulerAngles.ToDegrees(EulerAngles.ToUnityAngle(motion.Item1)));
        for (int i = 0; i < _Run.data.MotionsAngle.Count; i += (int)(0.02 / _Run.DeltaTime))
            Angle.Add(EulerAngles.ToDegrees(EulerAngles.ToUnityAngle(_Run.data.MotionsAngle[i].Item1)));

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
        _Run.save.SaveGame(_Run.data, _Run.odeMethod, 0.02, (int)(0.02 / _Run.DeltaTime));
        //Math.Round(0.019,);
        */
    }
    private void FixedUpdate()
    {
        //if (Time.time <= _Run.TimeEnd)
        //{
        //    LinearMotion();
        //    AttitudeMotion();
        //}
        //else
        //    EditorApplication.isPaused = true;
    }
    private void LinearMotion()
    {
        transform.position = _Planet.transform.position + new Vector3((float)(r * Math.Cos(Time.time * Mathf.Deg2Rad)), (float)(r * Math.Sin(Time.time * Mathf.Deg2Rad)), 0);
    }
    private void AttitudeMotion()
    {
        gameObject.transform.rotation = Quaternion.Euler(new Vector3((float)Angle[k].phi, (float)Angle[k].psi, (float)Angle[k].theta));
        k++;
    }
    private void CheckInputData()
    {

    }
}
