using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DATA;
using System.Numerics;
using Unity.VisualScripting;

public class Save
{
    private double time = 0;
    private readonly string Path = "C:/Users/svyatoslav/Desktop/Research work and research practice/Maple";
    private readonly string MethodClassic = "/Classic.txt";
    private readonly string Method3to8 = "/3_8.txt";
    private readonly string MethodFehlberg = "/Fehlberg.txt";
    public void SaveGame(Data data, Run.ODEMethod odeMethod, double deltaTime, int step)
    {
        //string filePath = Application.dataPath + MethodFehlberg;
        string filePath = Path;
        if (odeMethod == Run.ODEMethod.RungeKutta_Claccic)
            filePath += MethodClassic;
        else if (odeMethod == Run.ODEMethod.RungeKutta_3_8)
            filePath += Method3to8;
        else if (odeMethod == Run.ODEMethod.RungeKutta_Fehlberg45)
            filePath += MethodFehlberg;
        Debug.LogWarning(filePath);
        Debug.LogWarning($"{(data.MotionsAngle.Length - 1) / step + 1}");
        /*
        using var sw1 = new StreamWriter(filePath1);
        {
            foreach (var motionsAngle in data.MotionsAngle)
            {
                sw1.Write($"{(float)timeEnd}\timeEnd".Replace(",", "."));
                sw1.Write($"{(float)motionsAngle.Item1.phi}\timeEnd".Replace(",", "."));
                sw1.Write($"{(float)motionsAngle.Item1.psi}\timeEnd".Replace(",", "."));
                sw1.Write($"{(float)motionsAngle.Item1.theta}\timeEnd".Replace(",", "."));
                sw1.Write($"{(float)motionsAngle.Item2.pphi}\timeEnd".Replace(",", "."));
                sw1.Write($"{(float)motionsAngle.Item2.ppsi}\timeEnd".Replace(",", "."));
                sw1.WriteLine($"{(float)motionsAngle.Item2.ptheta}\timeEnd".Replace(",", "."));
                timeEnd = Math.Round(timeEnd + deltaTime, 3);
            }
        }
        timeEnd = 0;
        */
        using var sw = new StreamWriter(filePath);
        for (int i = 0; i < (data.MotionsAngle.Length - 1) / step + 1; i++)
        {
            sw.Write($"{(float)time}\t".Replace(",", "."));
            time = Math.Round(time + deltaTime, 3);
        }
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += step)
            sw.Write($"{(float)data.MotionsAngle[i].Item1.phi}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += step)
            sw.Write($"{(float)data.MotionsAngle[i].Item1.psi}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += step)
            sw.Write($"{(float)data.MotionsAngle[i].Item1.theta}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += step)
            sw.Write($"{(float)data.MotionsAngle[i].Item2.pphi}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i += step)
            sw.Write($"{(float)data.MotionsAngle[i].Item2.ppsi}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i++)
            sw.Write($"{(float)data.MotionsAngle[i].Item2.ptheta}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Length; i++)
            sw.Write($"{(float)data.H[i]}\t".Replace(",", "."));
    }

}
