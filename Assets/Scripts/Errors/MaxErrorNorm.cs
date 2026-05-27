using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts.Errors
{
    public class MaxErrorNorm : ErrorNormBase
    {
        public MaxErrorNorm(double atol, double rtol) : base(atol, rtol) { }

        public override double Compute(Vector<double> yNew, Vector<double> yLower)
            => ScaledError(yNew, yLower).AbsoluteMaximum();
    }
}
