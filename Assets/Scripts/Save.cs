using System.IO;
using UnityEngine;
using DATA;

public class Save
{
    private readonly string Path = "C:/Users/svyatoslav/Desktop/Research work and research practice/Maple";
    private readonly string RKF78 = "/RKF78.txt";
    private readonly string RKDP78 = "/RKDP78.txt";
    private readonly string RKF56 = "/RKF56.txt";
    private readonly string RKV56 = "/RKV56.txt";
    private readonly string RKDP45 = "/RKDP45.txt";
    private readonly string RKDP45_1 = "/RKDP45_1.txt";
    private readonly string RKF45 = "/RKF45.txt";
    private readonly string RKM = "/RKM.txt";
    private readonly string RKE = "/RKE.txt";
    private readonly string RKCK = "/RKCK.txt";
    private readonly string RKBS = "/RKBS.txt";
    private readonly string RK3_8 = "/RK3_8.txt";
    private readonly string RKClassic = "/RKClassic.txt";
    private readonly string RKT = "/RKT.txt";

    private readonly string EClassic = "/EClassic.txt";
    private readonly string EDDE = "/EDDE.txt";
    private readonly string EDM = "/EDM.txt";
    public void SaveGame(Data data, ODEMethod odeMethod, WaysSolveKeplerEquation waysSolveKeplerEquation)
    {
        //string filePathODE = Application.dataPath + RKF45;
        string filePathODE = Path;
        string filePathE = Path;

        if (odeMethod == ODEMethod.RungeKutta_DormandPrince_78)
            filePathODE += RKDP78;
        else if (odeMethod == ODEMethod.RungeKutta_Fehlberg_78)
            filePathODE += RKF78;
        else if (odeMethod == ODEMethod.RungeKutta_Fehlberg_56)
            filePathODE += RKF56;
        else if (odeMethod == ODEMethod.RungeKutta_Verner_56)
            filePathODE += RKV56;
        else if (odeMethod == ODEMethod.RungeKutta_DormandPrince_45)
            filePathODE += RKDP45;
        else if (odeMethod == ODEMethod.RungeKutta_DormandPrince_45_1)
            filePathODE += RKDP45_1;
        else if (odeMethod == ODEMethod.RungeKutta_Fehlberg_54)
            filePathODE += RKF45;
        else if (odeMethod == ODEMethod.RungeKutta_England_54)
            filePathODE += RKE;
        else if (odeMethod == ODEMethod.RungeKutta_CashKarp_54)
            filePathODE += RKCK;
        else if (odeMethod == ODEMethod.RungeKutta_BogackiShampine_45)
            filePathODE += RKBS;
        else if (odeMethod == ODEMethod.RungeKutta_Merson_45)
            filePathODE += RKM;
        else if (odeMethod == ODEMethod.RungeKutta_Claccic)
            filePathODE += RKClassic;
        else if (odeMethod == ODEMethod.RungeKutta_3_8)
            filePathODE += RK3_8;
        else
            filePathODE += RKT;

        if (waysSolveKeplerEquation == WaysSolveKeplerEquation.Iteration_method)
            filePathE += EClassic;
        else if (waysSolveKeplerEquation == WaysSolveKeplerEquation.DecompositionEccentricity)
            filePathE += EDDE;
        else
            filePathE += EDM;
        Debug.LogWarning(filePathODE);
        Debug.LogWarning(filePathE);
        using var swE = new StreamWriter(filePathE);
        {
            for (int i = 0; i < data.FlightTime.Length; i++)
                swE.Write($"{(float)data.FlightTime[i]}\t".Replace(",", "."));
            swE.WriteLine();
            for (int i = 0; i < data.E.Length; i++)
                swE.Write($"{(float)data.E[i]}\t".Replace(",", "."));
            swE.WriteLine();
            for (int i = 0; i < data.Nu.Length; i++)
                swE.Write($"{(float)data.Nu[i]}\t".Replace(",", "."));
            swE.WriteLine();
            for (int i = 0; i < data.NuAbs.Length; i++)
                swE.Write($"{(float)data.NuAbs[i]}\t".Replace(",", "."));
        }
        using var swODE = new StreamWriter(filePathODE);
        for (int i = 0; i < data.NuAbs.Length; i+=50)
            swODE.Write($"{(float)data.NuAbs[i]}\t".Replace(",", "."));//time = Math.Round(time + deltaTime, 3);
        swODE.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += 50)
            swODE.Write($"{(float)data.MotionsAngle[i].Item1.phi}\t".Replace(",", "."));
        swODE.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += 50)
            swODE.Write($"{(float)data.MotionsAngle[i].Item1.psi}\t".Replace(",", "."));
        swODE.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += 50)
            swODE.Write($"{(float)data.MotionsAngle[i].Item1.theta}\t".Replace(",", "."));
        swODE.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += 50)
            swODE.Write($"{(float)data.MotionsAngle[i].Item2.pphi}\t".Replace(",", "."));
        swODE.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += 50)
            swODE.Write($"{(float)data.MotionsAngle[i].Item2.ppsi}\t".Replace(",", "."));
        swODE.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += 50)
            swODE.Write($"{(float)data.MotionsAngle[i].Item2.ptheta}\t".Replace(",", "."));
        swODE.WriteLine();
        for (int i = 0; i < data.H.Length; i += 50)
            swODE.Write($"{(float)data.H[i]}\t".Replace(",", "."));
    }
}
