using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System;
using DATA;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using System.Numerics;
using TMPro;

public class Run : MonoBehaviour
{
    //[SerializeField] private double r_min;
    //[SerializeField] private RegularPrecessions _RegularPrecession;
    //[SerializeField] private ODEMethod methods = ODEMethod.RungeKutta_3_8;
    //[SerializeField] private int timeEnd = 90;
    //[SerializeField] private double MydeltaTime = 0.02;
    //[SerializeField] private int _scale = 1000000;
    public static Run Instance { get; set; }
    public const double G = 6670e-14;
    //satelite
    public double AB { get; private set; } = 1;// для прец по 1
    public double C { get; private set; } = 0.5;// для прец по 1
    public double Phi0 { get; private set; } = 0;
    public double Psi0 { get; private set; } = Math.PI / 6; // 0.52359877559829887307710723054658;
    public double Theta0 { get; private set; } = Math.PI / 6; // 1.0471975511965977461542144610932;
    public double Ppsi0 { get; private set; } = 0.2;
    public double Ptheta0 { get; private set; } = 0.2;
    public int SelectingParameter { get; private set; } = 0;
    public double ValueParameter { get; private set; } = 1; // для прец по 0,5 beta
    // Коническая
    //0.86602540378443864676372317075294
    // Болоид
    //-0.5
    public double r0 { get; private set; } = 0.1;
    public double Beta { get; private set; } = 0.5;
    public double PhiDot0 { get; private set; } = 0.1;
    public double Height { get; private set; } = 1000000;
    //planet
    public double e { get; private set; } = 0.01;
    public double R { get; private set; } = 6378000;
    public double M { get; private set; } = 59726;//5.976*10^(27-3)
    public int MPower { get; private set; } = 20;
    public double mu => G * M;
    // other
    public int TimeEnd { get; private set; } = 7200; // 86400;
    public double StepIntegration { get; private set; } = 0.00001;
    public double Epsilon { get; private set; } = 0.000001;//0.0001
    public int ApproximationNumberKeplerEquation { get; private set; } = 3;
    public int Scale { get; private set; } = 1000000;
    public ODEMethod odeMethod { get; private set; } = ODEMethod.RungeKutta_Fehlberg_78;
    public RegularPrecessions regularPrecession { get; private set; } = RegularPrecessions.MyParametrs;
    public WaysSolveKeplerEquation waysSolveKeplerEquation { get; private set; } = WaysSolveKeplerEquation.Denby;


    public Save save;
    public Data data;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        save = new Save();
        data = new(regularPrecession);
    }
    public void StartSimulation()
    {
        if (SelectingParameter == 0)
            Beta = ValueParameter;
        else if (SelectingParameter == 1)
            r0 = ValueParameter;
        else
            PhiDot0 = ValueParameter;
        if (CheckInputData())
        {
            M *= Math.Pow(10, MPower);
            SceneManager.LoadScene(1);//"Simulation", LoadSceneMode.Single
        }
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void NewAB(string value) => Instance.AB = double.Parse(value);
    //public void NewAB1(string value) => Instance.AB = double.Parse(value);
    public void NewC(string value) => Instance.C = double.Parse(value);
    public void NewValueParameter(string value) => Instance.ValueParameter = double.Parse(value);
    public void StartPhi(string value) => Instance.Phi0 = double.Parse(value);
    public void StartPsi(string value) => Instance.Psi0 = double.Parse(value);
    public void StartTheta(string value) => Instance.Theta0 = double.Parse(value);
    public void StartPPsi(string value) => Instance.Ppsi0 = double.Parse(value);
    public void StartPTheta(string value) => Instance.Ptheta0 = double.Parse(value);

    public void StartDistance(string value) => Instance.Height = double.Parse(value);
    public void RadiusPlanet(string value) => Instance.R = double.Parse(value);
    public void Eccentricity(string value) => Instance.e = double.Parse(value);
    public void MassPlanet(string value) => Instance.M = double.Parse(value);
    public void MassPlanetPower(string value) => Instance.MPower = int.Parse(value);

    public void NewTimeEnd(string value) => Instance.TimeEnd = int.Parse(value);
    public void NewStepIntegration(string value) => Instance.StepIntegration = double.Parse(value);
    public void NewApproximationNumberKeplerEquation(string value) => Instance.ApproximationNumberKeplerEquation = int.Parse(value);
    public void NewEpsilon(string value) => Instance.Epsilon = double.Parse(value);
    public void NewScale(string value) => Instance.Scale = int.Parse(value);
    public void NewODE(int value) => Instance.odeMethod = (ODEMethod)value;
    public void SelectParameter(int value) => Instance.SelectingParameter = value;
    public void NewRP(int value) => Instance.regularPrecession = (RegularPrecessions)value;
    public void NewTupeApr(int value) => Instance.waysSolveKeplerEquation = (WaysSolveKeplerEquation)value;
    public bool CheckInputData()
    {
        double alpha = Instance.C / Instance.AB;
        double omega0 = Math.Sqrt((G * M * Math.Pow(10, MPower)) * Math.Pow((1 - Instance.e) / (Instance.Height + Instance.R), 3));
        double beta;
        if (Instance.SelectingParameter == 0)
            beta = Instance.Beta;
        else if (Instance.SelectingParameter == 1)
            beta = Instance.r0 / omega0;
        else
            beta = (((Math.Sin(Theta0) * Math.Cos(Psi0) * Math.Cos(Theta0) * Math.Pow(1 + e, 2) - Ppsi0) * Math.Cos(Theta0) 
                - Instance.PhiDot0 * Math.Pow(Math.Sin(Theta0), 2) * Math.Pow(1 + e, 2)) * (e - 1) * Math.Sqrt(1 - Math.Pow(e, 2)) 
                - Math.Cos(Psi0) * omega0 * Math.Pow(1 + e, 3) * Math.Pow(Math.Sin(Theta0), 3))
                / (Math.Pow(1 + e, 2) * (1 - e) * (omega0 * Math.Pow(Math.Sin(Theta0), 2) 
                * Math.Sqrt(1 - Math.Pow(e, 2)) + alpha * Math.Pow(Math.Cos(Theta0), 2) * Math.Pow(1 - e, 2)));
        if (0 >= alpha || alpha > 2)
        {
            Debug.LogError($"alpha error: 0<={alpha}<2");
            return false;
        }
        if (Instance.regularPrecession == RegularPrecessions.Hyperboloidal && Math.Abs(alpha * beta) > 1)
        {
            Debug.LogError($"|Alpha * Beta| > 1 <=> {Math.Abs(alpha * beta)} > 1");
            Debug.LogWarning($"Alpha = C/A {alpha}");
            Debug.LogWarning($"Beta = r0/omega0 {beta}");
            Debug.LogWarning($"Omega0 = {omega0}, если много, увеличьте расстояние спутника");
            return false;
        }
        if (Instance.regularPrecession == RegularPrecessions.Conical && (Math.Abs(alpha * beta) > Math.Abs(3 * alpha - 4) || alpha == 4d / 3)) 
        {
            Debug.LogError($"|Alpha * Beta| > |3 * Alpha - 4| <=> {Math.Abs(alpha * beta)} > {Math.Abs(3 * alpha - 4)} или Alpha = 4 / 3 <=> {alpha} = {4d / 3}");
            Debug.LogWarning($"Alpha = C/A {alpha}");
            Debug.LogWarning($"Beta = r0/omega0 {beta}");
            Debug.LogWarning($"Omega0 = {omega0}, если много, увеличьте расстояние спутника");
            return false;
        }
        return true;
    }
}
