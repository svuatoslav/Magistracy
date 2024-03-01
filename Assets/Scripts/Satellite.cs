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
    //[SerializeField] private double _V0;
    [SerializeField] private double _r_min;
    [SerializeField] private double _r0;
    internal EulerAngles[] Angle;
    private int _index = 0;
    internal double A { get { return _A; } }
    internal double B { get { return _B; } }
    internal double C { get { return _C; } }
    //internal double V0 { get { return _V0; } }
    internal double q { get { return _r_min; } }
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
    internal double p
    {
        get
        {
            if (_Planet.Orbit == Planet.Orbits.Elliptical)
                return q * (1 + _Planet.Eccentricity);
            else
                return q;
            //if (_Planet.Orbit == Planet.Orbits.Elliptical)
            //    return StartPosition * (1 + _Planet.Eccentricity);
            //else
            //    return StartPosition;
        }
    }
    internal double SectorSpeed => Math.Sqrt(_Planet.mu * p);//(StartPosition * V0) / 2;
    internal double Omega0
    {
        get
        {
            if (_Planet.Orbit == Planet.Orbits.Elliptical)
                return 2 * Math.PI * Math.Sqrt(_Planet.mu * Math.Pow((1 - _Planet.Eccentricity) / q, 3));
            else
                return 2 * Math.PI * Math.Sqrt(_Planet.mu / Math.Pow(q, 3));
            //if (_Planet.Orbit == Planet.Orbits.Elliptical)
            //    return Math.Pow(V0, 3) / (2*Math.PI*_Planet.g * Math.Pow(_Planet.R, 2)) * Math.Pow((1 - _Planet.Eccentricity) / (1 + _Planet.Eccentricity), 3d / 2);
            //else
            //    return Math.Pow(V0, 3) / (2*Math.PI*_Planet.g * Math.Pow(_Planet.R, 2));
        }
    }
    internal double Alpha => C / A;
    internal double Beta => r0 / Omega0;
    private (EulerAngles, DimensionlessPulses) motion;
    private void Start()
    {
        CheckInputData();
        transform.position = new Vector3((float)((_Planet.StartPosition + q) / _Run.Scale), 0, 0);
        ///
        /// вычисления
        ///
        double[] flightTime = new double[(int)Math.Round(_Run.TimeEnd / 0.02, 0) + 1];
        for (int i = 1; i < flightTime.Length; i++)
            flightTime[i] = Math.Round(flightTime[i - 1] + 0.02,2);
        SolveKeplerEquation solveKeplerEquation = new ClassicMethodSuccessiveApproximations(_Planet.n, _Planet.Eccentricity);
        double[] E = solveKeplerEquation.Calculate(3, flightTime);
        _Run.data.Nu = new double[E.Length];
        _Run.data.H = new double[E.Length];
        Angle = new EulerAngles[E.Length];
        for (int i = 0; i < E.Length; i++)
        {
            _Run.data.Nu[i] = Elliptic.EtoNu(E[i], _Planet.Eccentricity);
            Debug.LogWarning($"Время {flightTime[i]} -> E {E[i] * Mathf.Rad2Deg} -> nu {_Run.data.Nu[i] * Mathf.Rad2Deg}");
        }
        _Run.data.MotionsAngle = _Run.solveDifferentialEquation.RKCalculate((new EulerAngles(Phi0, Psi0, Theta0), new DimensionlessPulses(Pphi0, Ppsi0, Ptheta0)), _Run.data.Nu, _Planet.ClassOrbit.ODEMotions);
        for (int i = 0; i < E.Length; i++)
        {
            _Planet.ClassOrbit.H(_Run.data.Nu[i], _Run.data.MotionsAngle[i]);
            Angle[i] = (EulerAngles.ToDegrees(EulerAngles.ToUnityAngle(_Run.data.MotionsAngle[i].Item1)));
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
        //_Run.save.SaveGame(_Run.data, _Run.odeMethod, 0.02, (int)(0.02 / _Run.DeltaTime));
        //Math.Round(0.019,);
        
    }
    private void FixedUpdate()
    {
        if (Time.time <= _Run.TimeEnd)
        {
            LinearMotion();
            AttitudeMotion();
            _index++;
            Debug.Log($"время {Time.time}, индекс {_index++}, угол {_Run.data.Nu[_index]}");
        }
        else
            EditorApplication.isPaused = true;
    }
    internal double r(double nu) => p / (1 + _Planet.Eccentricity * Math.Cos(nu));
    private void LinearMotion()
    {
        transform.position = _Planet.transform.position + new Vector3((float)(r(_Run.data.Nu[_index]) * Math.Cos(_Run.data.Nu[_index]) / _Run.Scale), (float)(r(_Run.data.Nu[_index]) * Math.Sin(_Run.data.Nu[_index]) / _Run.Scale), 0);
    }
    private void AttitudeMotion()
    {
        gameObject.transform.rotation = Quaternion.Euler(new Vector3((float)Angle[_index].phi, (float)Angle[_index].psi, (float)Angle[_index].theta));
    }
    private void CheckInputData()
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
        //if (V0 >= Math.Sqrt(_Planet.g * _Planet.R * (1 + _Planet.Eccentricity)))
        //{
        //    Debug.LogError("V0 error");
        //    EditorApplication.isPaused = true;
        //}
        //Debug.Log($"V0 = {V0} < {Math.Sqrt(_Planet.g * _Planet.R * (1 + _Planet.Eccentricity))}");

        if (!(_Planet.R < q))
        {
            Debug.LogError("расстояние спутника от центра земли слишком мало");
            EditorApplication.isPaused = true;
        }
        Debug.Log($"Omega0 = {Omega0}");
    }
}
