using System;
using System.Linq;

public abstract class KeplerEquationSolver
{
    protected readonly double n;
    protected readonly double Eccentricity;
    protected double t0 = 0;

    public KeplerEquationSolver(double n, double eccentricity)
    {
        this.n = n;
        Eccentricity = eccentricity;
    }

    protected double M(double t) => n * (t - t0);

    public abstract double E(double t, double En);

    public abstract double E0(double t);

    protected int Factorial(int number) => Enumerable.Range(1, number).Aggregate((x, y) => x * y);

    public double[] Calculate(int approximation, double[] time)
    {
        double[] result = new double[time.Length];

        for (int i = 0; i < result.Length; i++)
            result[i] = E0(time[i]);

        for (int i = 1; i <= approximation; i++)
            for (int j = 0; j < result.Length; j++)
                result[j] = E(time[j], result[j]);

        return result;
    }

    public double[] Calculate(double epsilon, double[] time)
    {
        double[] previous = new double[time.Length];
        double[] current = new double[time.Length];

        for (int i = 0; i < time.Length; i++)
            previous[i] = E0(time[i]);

        bool converged;
        do
        {
            (previous, current) = (current, previous);
            converged = true;

            for (int i = 0; i < time.Length; i++)
            {
                current[i] = E(time[i], previous[i]);
                if (Math.Abs(previous[i] - current[i]) > epsilon)
                {
                    converged = false;
                    break;
                }
            }
        }
        while (!converged);

        return current;
    }
}
