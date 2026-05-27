using MathNet.Numerics.LinearAlgebra;
using System;

namespace Assets.Scripts.RungeKutta
{
    public class ExplicitRungeKutta : RungeKuttaBase
    {
        private ButcherTableau _butcherTableau;

        public ExplicitRungeKutta(ButcherTableau butcherTableau)
        {
            _butcherTableau = butcherTableau;
        }

        public override (double[] y, double[] t) UniversalOrder(
            double y0,
            double t0,
            double dt,
            double tEnd,
            Func<double, double, double> function)
        {
            if (!IsValidTime(t0, dt, tEnd))
                throw new ArgumentException("Некоректные временные значения");

            int startVectorPlace = 1;
            double[] y = new double[(int)((tEnd - t0) / dt) + startVectorPlace];
            double[] t = new double[y.Length];

            y[0] = y0;
            t[0] = t0;
            for (int i = 1; i < y.Length; i++)
                (y[i], t[i]) = Step(y[i - 1], t[i - 1], dt, function);

            return (y, t);
        }

        public (double y, double t) Step(
            double y0,
            double t0,
            double dt,
            Func<double, double, double> f)
        {
            var k = new double[_butcherTableau.C.Length];
            double resultWeightedPrevStages = 0;
            double sumWeightedSlopes = 0;

            for (int i = 0; i < k.Length; i++)
            {
                resultWeightedPrevStages = 0;

                for (int j = 0; j < i; j++)
                    resultWeightedPrevStages += _butcherTableau.A[i, j] * k[j];

                k[i] = f(t0 + dt * _butcherTableau.C[i], y0 + dt * resultWeightedPrevStages);

                sumWeightedSlopes += _butcherTableau.B[i] * k[i];
            }

            return (y0 + dt * sumWeightedSlopes, t0 + dt);
        }

        public override (Vector<double>[] y, double[] t) UniversalOrder(
            Vector<double> y0,
            double t0,
            double dt,
            double tEnd,
            Func<double, Vector<double>, Vector<double>> function)
        {
            if (!IsValidTime(t0, dt, tEnd))
                throw new ArgumentException("Некоректные временные значения");

            int startVectorPlace = 1;
            Vector<double>[] y = new Vector<double>[(int)((tEnd - t0) / dt) + startVectorPlace];
            double[] t = new double[y.Length];

            y[0] = y0;
            t[0] = t0;
            for (int i = 1; i < y.Length; i++)
                (y[i], t[i]) = Step(y[i - 1], t[i - 1], dt, function);

            return (y, t);
        }

        public (Vector<double> y, double t) Step(
            Vector<double> y0,
            double t0,
            double dt,
            Func<double, Vector<double>, Vector<double>> f)
        {
            var k = new Vector<double>[_butcherTableau.C.Length];
            Vector<double> resultWeightedPrevStages = Vector<double>.Build.Dense(y0.Count);
            Vector<double> sumWeightedSlopes = Vector<double>.Build.Dense(y0.Count);

            for (int i = 0; i < k.Length; i++)
            {
                resultWeightedPrevStages = Vector<double>.Build.Dense(y0.Count);

                for (int j = 0; j < i; j++)
                    resultWeightedPrevStages += _butcherTableau.A[i, j] * k[j];

                k[i] = f(t0 + dt * _butcherTableau.C[i], y0 + dt * resultWeightedPrevStages);

                sumWeightedSlopes += _butcherTableau.B[i] * k[i];
            }

            return (y0 + dt * sumWeightedSlopes, t0 + dt);
        }
    }
}
