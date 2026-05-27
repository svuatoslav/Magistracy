using System;
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts.Errors
{
    public abstract class ErrorNormBase : IErrorNorm
    {
        protected readonly double Atol;

        protected readonly double Rtol;

        public ErrorNormBase(double atol, double rtol)
        {
            Atol = atol; 
            Rtol = rtol;
        }

        public abstract double Compute(Vector<double> yNew, Vector<double> yLower);

        public double Compute(double yNew, double yLower)
        {
            double error = yNew - yLower;
            double scale = Atol + Rtol * Math.Abs(yNew);
            return Math.Abs(error / scale);
        }

        protected double ScaledError(double yNew, double yLower)
        {
            var error = yNew - yLower;
            var scale = Atol + Rtol * Math.Abs(yNew);
            return error / scale;
        }

        protected Vector<double> ScaledError(Vector<double> yNew, Vector<double> yLower)
        {
            var error = yNew - yLower;
            var scale = yNew.Map(y => Atol + Rtol * Math.Abs(y));
            return error.PointwiseDivide(scale);
        }
    }
}
