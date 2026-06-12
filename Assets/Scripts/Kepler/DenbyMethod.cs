using static System.Math;

public sealed class DenbyMethod : KeplerEquationSolver
{
    public DenbyMethod(double n, double e) : base(n, e) { }

    public override double[] Calculate(int approximation, double[] time)
    {
        double[] result = new double[time.Length];
        NumberApproximation = approximation;
        switch (NumberApproximation)
        {
            case 0:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E0(time[i]);
                return result;
            case 1:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E1(time[i]);
                return result;
            default:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E1(time[i]);
                for (approximation = 2; approximation <= NumberApproximation; approximation++)
                    for (int i = 0; i < result.Length; i++)
                    {
                        if (result[i] - 2 * (M(time[i]) + e * Sin(result[i])) + M(time[i]) + e * Sin(M(time[i]) + e * Sin(result[i])) == 0)
                            continue;
                        result[i] = E(time[i], result[i]);
                    }
                return result;
        }
    }

    public override double E(double t, double En) => En - Pow(M(t) + e * Sin(En) - En, 2) / (En - 2 * (M(t) + e * Sin(En)) + M(t) + e * Sin(M(t) + e * Sin(En)));
    public override double E0(double t) => M(t) + 0.85 * e;
    public override double E1(double t) => M(t) + e * (0.85 - Pow(Sin(M(t) + 0.85 * e) - 0.85, 2) / (0.85 - 2 * Sin(M(t) + 0.85 * e) + Sin(M(t) + e * Sin(M(t) + 0.85 * e))));
}
