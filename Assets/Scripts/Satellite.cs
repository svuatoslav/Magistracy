using Assets.Scripts;
using System;
using UnityEngine;

public class Satellite : MonoBehaviour
{
    [SerializeField] private Planet _planet = null;
    [SerializeField] private Orbit _orbit = null;
    [SerializeField] private TimeUI _UISimulation = null;

    [SerializeField] private double _AB = 0;
    [SerializeField] private double _C = 0;
    [SerializeField] private double _alpha = 0;

    [SerializeField] private double _r0 = 0;
    [SerializeField] private double _beta = 0;

    [SerializeField] private double _phi;
    [SerializeField] private double _psi;
    [SerializeField] private double _theta;
    [SerializeField] private double _pphi;
    [SerializeField] private double _ppsi;
    [SerializeField] private double _ptheta;

    private int _timeIndex = 0;
    //public EulerAngles[] Angle;
    public double AB => _AB;
    public double C => _C;
    public double Alpha => _alpha;
    public double Beta => _beta;
    public double Phi0 { get; private set; }
    public double Psi0 { get; private set; }
    public double Theta0 { get; private set; }
    public double Pphi0 { get; private set; }
    public double Ppsi0 { get; private set; }
    public double Ptheta0 { get; private set; }

    public double Phi => _phi;
    public double Psi => _psi;
    public double Theta => _theta;
    public double Pphi => _pphi;
    public double Ppsi => _ppsi;
    public double Ptheta => _ptheta;

    public void ApplyConfig(SatelliteConfig satelliteConfig, Planet planet, Orbit orbit)
    {
        Phi0 = satelliteConfig.Phi;
        Psi0= satelliteConfig.Psi;
        Theta0 = satelliteConfig.Theta;
        Pphi0 = satelliteConfig.Pphi;
        Ppsi0 = satelliteConfig.Ppsi;
        Ptheta0 = satelliteConfig.Theta;

        _AB = satelliteConfig.AB;
        _C = satelliteConfig.C;
        _r0 = satelliteConfig.r0;

        _alpha = satelliteConfig.C / satelliteConfig.AB;
        _beta = satelliteConfig.r0 / orbit.Omega0;

        _planet = planet;
        _orbit = orbit;
    }

    private void Start()
    {
        //Angle
        transform.eulerAngles = Vector3.zero;

        Vector3 startPosition = new(0, 0, (float)(_orbit is Elliptic elliptic ? elliptic.R_p : ((Circular)_orbit).R));
        transform.position = _planet.transform.position + startPosition; 
    }

    private void FixedUpdate()
    {
        //if (true)
        //{
        //    transform.position = LinearMotion();
        //    gameObject.transform.rotation = AttitudeMotion();

        //    _UISimulation.UITime(_timeIndex);
        //    ChangeTime();
        //}
    }

    private void ChangeTime()
    {
        //if (_timeIndex + SpeedTime < Angle.Length)
        //    _timeIndex += SpeedTime;
        //else if (_timeIndex < Angle.Length - 1)
        //    _timeIndex++;
    }

    //private double r(double nu) => p / (1 + _orbit.Eccentricity * Math.Cos(nu));

    //private Vector3 LinearMotion() => _planet.transform.position + new Vector3(
    //    (float)(-r(Run.Instance.data.Nu[_timeIndex]) * Math.Sin(Run.Instance.data.Nu[_timeIndex])),
    //    0, 
    //    (float)(r(Run.Instance.data.Nu[_timeIndex]) * Math.Cos(Run.Instance.data.Nu[_timeIndex])));//  / Run.Instance.Scale

    //private Quaternion AttitudeMotion() => Quaternion.Euler(new Vector3((float)Angle[_timeIndex].phi, (float)(Angle[_timeIndex].psi - Run.Instance.data.Nu[_timeIndex] * Mathf.Rad2Deg), (float)Angle[_timeIndex].theta));
}
