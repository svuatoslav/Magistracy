using System;
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts.RungeKutta
{
    public interface IRungeKutta
    {
        (double[] y, double[] t) UniversalOrder(
            double y0,
            double t0,
            double dt,
            double tEnd,
            Func<double, double, double> f);

        (Vector<double>[] y, double[] t) UniversalOrder(
            Vector<double> y0,
            double t0,
            double dt,
            double tEnd,
            Func<double, Vector<double>, Vector<double>> f);
    }
}
