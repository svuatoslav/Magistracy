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
    //[SerializeField] private double q;
    //[SerializeField] private RegularPrecessions _RegularPrecession;
    //[SerializeField] private ODEMethod methods = ODEMethod.RungeKutta_3_8;
    //[SerializeField] private int timeEnd = 90;
    //[SerializeField] private double MydeltaTime = 0.02;
    //[SerializeField] private int _scale = 1000000;
    public static Run Instance { get; set; }
    public const double G = 6670e-14;
    //satelite
    public double AB { get; private set; }
    public double C { get; private set; }
    public double Phi0 { get; private set; }
    public double Psi0 { get; private set; }
    public double Theta0 { get; private set; }
    public double Pphi0 { get; private set; }
    public double Ppsi0 { get; private set; }
    public double Ptheta0 { get; private set; }
    public double r0 { get; private set; }
    public double Height { get; private set; }
    //planet
    public double e { get; private set; }
    public double R { get; private set; }
    public double M { get; private set; }//5.976*10^(27-3)
    public double mu => G * M;
    public int MPower { get; private set; }
    // other
    public int TimeEnd { get; private set; }
    public double StepIntegration { get; private set; }
    public double Epsilon { get; private set; }
    public int ApproximationNumberKeplerEquation { get; private set; }
    public int Scale { get; private set; }
    public ODEMethod odeMethod { get; private set; }
    public RegularPrecessions regularPrecession { get; private set; }
    public WaysSolveKeplerEquation waysSolveKeplerEquation { get; private set; }


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
    public void New_r0(string value) => Instance.r0 = double.Parse(value);
    public void StartPhi(string value) => Instance.Phi0 = double.Parse(value);
    public void StartPsi(string value) => Instance.Psi0 = double.Parse(value);
    public void StartTheta(string value) => Instance.Theta0 = double.Parse(value);
    public void StartPPhi(string value) => Instance.Pphi0 = double.Parse(value);
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
    public void NewRP(int value) => Instance.regularPrecession = (RegularPrecessions)value;
    public void NewTupeApr(int value) => Instance.waysSolveKeplerEquation = (WaysSolveKeplerEquation)value;
    public bool CheckInputData()
    {
        double alpha = Instance.C / Instance.AB;
        double omega0 = 2 * Math.PI * Math.Sqrt((G * M * Math.Pow(10, MPower)) * Math.Pow((1 - Instance.e) / (Instance.Height + Instance.R), 3));
        double beta = Instance.r0 / omega0;
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
