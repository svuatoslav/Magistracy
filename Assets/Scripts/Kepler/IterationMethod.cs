using System;

public sealed class IterationMethod : KeplerEquationSolver
{
    public IterationMethod(double n, double e) : base(n, e) { }

    public override double E(double t, double En) => M(t) + Eccentricity * Math.Sin(En);

    public override double E0(double t) => M(t);
}
