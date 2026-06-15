using Assets.Scripts.DataSerialiizers;
using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts.Storages
{
    public class ResultSimulation : IStorage
    {
        private readonly IDataManager _dataManager;
        private string _path;

        private double[] _time;
        private double[] _nu;
        private double[] _E;

        private Vector<double>[] _satelliteMovement;
        private double[] _H;

        public ResultSimulation(IDataManager dataManager, double[] time, double[] nu, double[] E, Vector<double>[] satelliteMovement, double[] H)
        {
            _path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\", "Data", "ResultSimulation"));
            _dataManager = dataManager;
            _time = time;
            _nu = nu;
            _E = E;
            _satelliteMovement = satelliteMovement;
            _H = H;
        }

        public void Save()
        {
            throw new NotImplementedException();
            //using var sw = new StreamWriter(FullPath);
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
    }
}
