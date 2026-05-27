using System;
using Assets.Scripts.Errors;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts.RungeKutta
{
    public class AdaptiveRungeKutta : RungeKuttaBase
    {
        private IErrorNorm _errorNorm;

        private EmbeddedButcherTableau _embeddedButcherTableau;

        private double _safetyFactor = 0.9;

        public AdaptiveRungeKutta(EmbeddedButcherTableau embeddedButcherTableau, IErrorNorm errorNorm)
        {
            if (errorNorm is null)
                throw new ArgumentException("Нет способа вычисления нормы ошибки");

            _embeddedButcherTableau = embeddedButcherTableau;
            _errorNorm = errorNorm;
        }

        public override (double[] y, double[] t) UniversalOrder(double y0, double t0, double dt, double tEnd, Func<double, double, double> f)
        {
            if (!IsValidTime(t0, dt, tEnd))
                throw new ArgumentException("Некоректные временные значения");

            int startVectorPlace = 1;
            var y = new List<double>((int)((tEnd - t0) / dt) + startVectorPlace);
            var t = new List<double>(y.Capacity);
            var errors = new List<double>(y.Capacity);


            var yCurrent = y0;
            var tCurrent = t0;
            double errorCurrent;

            while (tCurrent < tEnd)
            {
                if (tCurrent + dt > tEnd)
                    dt = tEnd - tCurrent;

                (yCurrent, tCurrent, errorCurrent) = Step(yCurrent, tCurrent, ref dt, f);

                y.Add(yCurrent);
                t.Add(tCurrent);
                errors.Add(errorCurrent);
            }

            return (y.ToArray(), t.ToArray());
        }

        public (double y, double t, double eror) Step(
            double y0,
            double t0,
            ref double dt,
            Func<double, double, double> f)
        {
            double yNew;
            double yLower;

            do
            {
                var k = new double[_embeddedButcherTableau.C.Length];
                var sumWeightedSlopes = 0.0;
                var sumWeightedSlopesLower = 0.0;

                for (int i = 0; i < k.Length; i++)
                {
                    var resultWeightedPrevStages = 0.0;

                    for (int j = 0; j < i; j++)
                        resultWeightedPrevStages += _embeddedButcherTableau.A[i, j] * k[j];

                    k[i] = f(t0 + dt * _embeddedButcherTableau.C[i], y0 + dt * resultWeightedPrevStages);

                    sumWeightedSlopes += _embeddedButcherTableau.B[i] * k[i];

                    sumWeightedSlopesLower += _embeddedButcherTableau.BLower[i] * k[i];
                }

                yNew = y0 + dt * sumWeightedSlopes;
                yLower = y0 + dt * sumWeightedSlopesLower;
            }
            while (!TryAcceptStep(yNew, yLower, ref dt));

            return (yNew, t0 + dt, yNew - yLower);
        }

        private bool TryAcceptStep(double yNew, double yLower, ref double dt)
        {
            var errorNorm = _errorNorm.Compute(yNew, yLower);

            dt *= _safetyFactor * Math.Pow(1.0 / errorNorm, 1.0 / _embeddedButcherTableau.Order);

            return errorNorm < 1.0;
        }

        public override (Vector<double>[] y, double[] t) UniversalOrder(
            Vector<double> y0,
            double t0,
            double dt,
            double tEnd,
            Func<double, Vector<double>, Vector<double>> f)
        {
            if (!IsValidTime(t0, dt, tEnd))
                throw new ArgumentException("Некоректные временные значения");

            int startVectorPlace = 1;
            var y = new List<Vector<double>>((int)((tEnd - t0) / dt) + startVectorPlace);
            var t = new List<double>(y.Capacity);
            var errors = new List<Vector<double>>(y.Capacity);

            var yCurrent = y0;
            var tCurrent = t0;
            Vector<double> error;

            while (tCurrent < tEnd)
            {
                if (tCurrent + dt > tEnd)
                    dt = tEnd - tCurrent;

                (yCurrent, tCurrent, error) = Step(yCurrent, tCurrent, ref dt, f);

                y.Add(yCurrent);
                t.Add(tCurrent);
                errors.Add(error);
            }

            return (y.ToArray(), t.ToArray());
        }

        public (Vector<double> y, double t, Vector<double> eror) Step(Vector<double> y0, double t0, ref double dt, Func<double, Vector<double>, Vector<double>> f)
        {
            Vector<double> yNew;
            Vector<double> yLower;

            do
            {
                var k = new Vector<double>[_embeddedButcherTableau.C.Length];
                var sumWeightedSlopes = Vector<double>.Build.Dense(y0.Count);
                var sumWeightedSlopesLower = Vector<double>.Build.Dense(y0.Count);

                for (int i = 0; i < k.Length; i++)
                {
                    var resultWeightedPrevStages = Vector<double>.Build.Dense(y0.Count);

                    for (int j = 0; j < i; j++)
                        resultWeightedPrevStages += _embeddedButcherTableau.A[i, j] * k[j];

                    k[i] = f(t0 + dt * _embeddedButcherTableau.C[i], y0 + dt * resultWeightedPrevStages);

                    sumWeightedSlopes += _embeddedButcherTableau.B[i] * k[i];

                    sumWeightedSlopesLower += _embeddedButcherTableau.BLower[i] * k[i];
                }

                yNew = y0 + dt * sumWeightedSlopes;
                yLower = y0 + dt * sumWeightedSlopesLower;
            }
            while (!TryAcceptStep(yNew, yLower, ref dt));

            return (yNew, t0 + dt, yNew - yLower);
        }

        private bool TryAcceptStep(Vector<double> yNew, Vector<double> yLower, ref double dt)
        {
            var errorNorm = _errorNorm.Compute(yNew, yLower);

            dt = dt * _safetyFactor * Math.Pow(1.0 / errorNorm, 1.0 / _embeddedButcherTableau.Order);

            return errorNorm < 1.0;
        }
    }
}
