using System;
using static System.Math;

public sealed class DecompositionDegreesEccentricity : KeplerEquationSolver 
{
    private const double _LaplaceLimitOfEccentricity = 0.6627434193493;
    private int _numberApproximation = 0; // пересмотреть решение позже

    public DecompositionDegreesEccentricity(double n, double e, int numberApproximation) : base(n, e) 
    {
        if (e >= _LaplaceLimitOfEccentricity)
            throw new Exception("The eccentricity is greater than the maximum value.");

        _numberApproximation = numberApproximation; 
    }

    public override double E(double t, double En) => En + Pow(Eccentricity, _numberApproximation) * AlgebraicSum(t) / Pow(2, _numberApproximation - 1);

    public override double E0(double t) => M(t);

    private double AlgebraicSum(double t)
    {
        double sum = 0;
        for (int s = 0; s <= _numberApproximation / 2; s++)
            sum += Pow(-1, s) * Pow(_numberApproximation - 2 * s, _numberApproximation - 1) / (Factorial(s) * Factorial(_numberApproximation - s)) * Sin((_numberApproximation - 2 * s) * M(t));
        return sum;
    }
}
