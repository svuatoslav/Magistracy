using System;
using static System.Math;

public sealed class DenbyMethod : KeplerEquationSolver
{
    public DenbyMethod(double n, double e) : base(n, e) { }

    public override double E(double t, double En)
    {
        if (CheckDivisor(t, En, out var divisor))
            return En - Pow(M(t) + Eccentricity * Sin(En) - En, 2) / divisor;

        throw new Exception("что-то пошло не так в методе Денди");
    }

    public override double E0(double t) => M(t) + 0.85 * Eccentricity;

    private bool CheckDivisor(double t, double En, out double divisor)
    {
        divisor = En - 2 * (M(t) + Eccentricity * Sin(En)) + M(t) + Eccentricity * Sin(M(t) + Eccentricity * Sin(En));

        if (divisor == 0)
            throw new DivideByZeroException("Ошибка деления в методе Денди при вычислении эксцентрической аномалии");

        return true;
    }
}
