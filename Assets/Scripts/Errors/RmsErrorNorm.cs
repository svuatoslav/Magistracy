using MathNet.Numerics.LinearAlgebra;
using System;

namespace Assets.Scripts.Errors
{
    public class RmsErrorNorm : ErrorNormBase
    {
        public RmsErrorNorm(double atol, double rtol) : base(atol, rtol) { }

        public override double Compute(Vector<double> yNew, Vector<double> yLower)
        {
            var ratios = ScaledError(yNew, yLower);
            return Math.Sqrt(ratios.DotProduct(ratios) / yNew.Count);
        }
    }
}
