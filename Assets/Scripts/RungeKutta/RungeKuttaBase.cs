using System;
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts.RungeKutta
{
    public abstract class RungeKuttaBase : IRungeKutta
    {
        public abstract (double[] y, double[] t) UniversalOrder(
            double y0,
            double t0,
            double dt,
            double tEnd,
            Func<double, double, double> f);

        public abstract (Vector<double>[] y, double[] t) UniversalOrder(
            Vector<double> y0,
            double t0,
            double dt,
            double tEnd,
            Func<double, Vector<double>, Vector<double>> f);

        private protected static bool IsValidTime(double t0, double dt, double tEnd) => tEnd > t0 && tEnd >= t0 + dt && dt > 0 && t0 > 0;
    }
}
