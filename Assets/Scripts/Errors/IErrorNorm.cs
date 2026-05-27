using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts.Errors
{
    public interface IErrorNorm
    {
        public double Compute(double yNew, double yLower);

        public double Compute(Vector<double> yNew, Vector<double> yLower);
    }
}
