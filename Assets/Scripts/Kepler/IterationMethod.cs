using static System.Math;

public sealed class IterationMethod : KeplerEquationSolver
{
    public IterationMethod(double n, double e) : base(n, e) { }

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
            case 2:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E2(time[i]);
                return result;
            case 3:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E3(time[i]);
                return result;
            default:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E3(time[i]);
                for (approximation = 4; approximation <= NumberApproximation; approximation++)
                    for (int i = 0; i < result.Length; i++)
                        result[i] = E(time[i], result[i]);
                return result;
        }
    }

    public override double E(double t, double En) => M(t) + e * Sin(En);
    public override double E0(double t) => M(t);
    public override double E1(double t) => M(t) + e * Sin(M(t));
    public double E2(double t) => M(t) + e * Sin(M(t) + e * Sin(M(t)));
    public double E3(double t) => M(t) + e * Sin(M(t) + e * Sin(M(t) + e * Sin(M(t))));
}
