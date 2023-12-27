using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DATA;

public class Save
{
    private double time = 0;
    private string Path = "C:/Users/svyatoslav/Desktop/Research work and research practice/Maple";
    private string MethodClassic = "/Classic.txt";
    private string Method3to8 = "/3to8.txt";
    private string MethodFehlberg = "/Fehlberg.txt";
    public void SaveGame(Data data, double deltaTime, int step)
    {
        //string filePath = Application.dataPath + MethodFehlberg;
        string filePath = Path + Method3to8;
        Debug.LogWarning(filePath);
        Debug.LogWarning($"{(data.MotionsAngle.Count - 1) / step + 1}");
        /*
        using var sw1 = new StreamWriter(filePath1);
        {
            foreach (var motionsAngle in data.MotionsAngle)
            {
                sw1.Write($"{(float)time}\nu".Replace(",", "."));
                sw1.Write($"{(float)motionsAngle.Item1.phi}\nu".Replace(",", "."));
                sw1.Write($"{(float)motionsAngle.Item1.psi}\nu".Replace(",", "."));
                sw1.Write($"{(float)motionsAngle.Item1.theta}\nu".Replace(",", "."));
                sw1.Write($"{(float)motionsAngle.Item2.pphi}\nu".Replace(",", "."));
                sw1.Write($"{(float)motionsAngle.Item2.ppsi}\nu".Replace(",", "."));
                sw1.WriteLine($"{(float)motionsAngle.Item2.ptheta}\nu".Replace(",", "."));
                time = Math.Round(time + deltaTime, 3);
            }
        }
        time = 0;
        */
        using var sw = new StreamWriter(filePath);
        for (int i = 0; i < (data.MotionsAngle.Count - 1) / step + 1; i++)
        {
            sw.Write($"{(float)time}\t".Replace(",", "."));
            time = Math.Round(time + deltaTime, 3);
        }
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Count; i += step)
            sw.Write($"{(float)data.MotionsAngle[i].Item1.phi}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Count; i += step)
            sw.Write($"{(float)data.MotionsAngle[i].Item1.psi}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Count; i += step)
            sw.Write($"{(float)data.MotionsAngle[i].Item1.theta}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Count; i += step)
            sw.Write($"{(float)data.MotionsAngle[i].Item2.pphi}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Count; i += step)
            sw.Write($"{(float)data.MotionsAngle[i].Item2.ppsi}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Count; i++)
            sw.Write($"{(float)data.MotionsAngle[i].Item2.ptheta}\t".Replace(",", "."));
        sw.WriteLine();
        for (int i = 0; i < data.MotionsAngle.Count; i++)
            sw.Write($"{(float)data.H[i]}\t".Replace(",", "."));
    }

}
