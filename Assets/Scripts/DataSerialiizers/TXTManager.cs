using System;
using System.IO;

namespace Assets.Scripts.DataSerialiizers
{
    public class TXTManager : IDataManager
    {
        private readonly string _format = ".txt";

        public T Read<T>(string path) where T : class
        {
            try
            {
                return null;
            }
            catch (DirectoryNotFoundException) { return null; }
            catch (FileNotFoundException) { return null; }
            catch (Exception) { return null; }
        }

        public void Write<T>(T data, string path) where T : class
        {
            try
            {
                //using var swE = new StreamWriter(filePathE);
                //{
                //    for (int i = 0; i < data.FlightTime.Length; i++)
                //        swE.Write($"{(float)data.FlightTime[i]}\t".Replace(",", "."));
                //    swE.WriteLine();
                //    for (int i = 0; i < data.E.Length; i++)
                //        swE.Write($"{(float)data.E[i]}\t".Replace(",", "."));
                //    swE.WriteLine();
                //    for (int i = 0; i < data.Nu.Length; i++)
                //        swE.Write($"{(float)data.Nu[i]}\t".Replace(",", "."));
                //    swE.WriteLine();
                //    for (int i = 0; i < data.NuAbs.Length; i++)
                //        swE.Write($"{(float)data.NuAbs[i]}\t".Replace(",", "."));
                //}
                //using var swODE = new StreamWriter(filePathODE);
                //for (int i = 0; i < data.NuAbs.Length; i += 50)
                //    swODE.Write($"{(float)data.NuAbs[i]}\t".Replace(",", "."));//time = Math.Round(time + deltaTime, 3);
                //swODE.WriteLine();
                //for (int i = 0; i < data.MotionsAngle.Length; i += 50)
                //    swODE.Write($"{(float)data.MotionsAngle[i].Item1.phi}\t".Replace(",", "."));
                //swODE.WriteLine();
                //for (int i = 0; i < data.MotionsAngle.Length; i += 50)
                //    swODE.Write($"{(float)data.MotionsAngle[i].Item1.psi}\t".Replace(",", "."));
                //swODE.WriteLine();
                //for (int i = 0; i < data.MotionsAngle.Length; i += 50)
                //    swODE.Write($"{(float)data.MotionsAngle[i].Item1.theta}\t".Replace(",", "."));
                //swODE.WriteLine();
                //for (int i = 0; i < data.MotionsAngle.Length; i += 50)
                //    swODE.Write($"{(float)data.MotionsAngle[i].Item2.pphi}\t".Replace(",", "."));
                //swODE.WriteLine();
                //for (int i = 0; i < data.MotionsAngle.Length; i += 50)
                //    swODE.Write($"{(float)data.MotionsAngle[i].Item2.ppsi}\t".Replace(",", "."));
                //swODE.WriteLine();
                //for (int i = 0; i < data.MotionsAngle.Length; i += 50)
                //    swODE.Write($"{(float)data.MotionsAngle[i].Item2.ptheta}\t".Replace(",", "."));
                //swODE.WriteLine();
                //for (int i = 0; i < data.H.Length; i += 50)
                //    swODE.Write($"{(float)data.H[i]}\t".Replace(",", "."));
            }
            catch (DirectoryNotFoundException) { }
            catch (InvalidOperationException) { }
            catch (Exception) { }
        }
    }
}
