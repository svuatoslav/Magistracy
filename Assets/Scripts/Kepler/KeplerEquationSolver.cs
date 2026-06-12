using System;
using System.Linq;

public abstract class KeplerEquationSolver
{
    protected readonly double n;
    protected readonly double e;
    protected int NumberApproximation;
    protected double Epsilon;
    protected double t0 = 0;
    public abstract double E(double t, double En);

    public KeplerEquationSolver(double n, double e)
    {
        this.n = n;
        this.e = e;
    }

    protected double M(double t) => n * (t - t0);

    public abstract double E0(double t);

    public abstract double E1(double t);

    protected int Factorial(int number) => Enumerable.Range(1, number).Aggregate((x, y) => x * y);

    public abstract double[] Calculate(int approximation, double[] time);

    public double[] Calculate(double epsilon, double[] time)
    {
        Epsilon = epsilon;
        double[] resultTemp = new double[time.Length];
        double[] result = new double[time.Length];
        for (int i = 0; i < result.Length; i++)
        {
            resultTemp[i] = E0(time[i]);
            result[i] = E1(time[i]);
            if (Math.Abs(resultTemp[i] - result[i]) > epsilon)
                break;
            else if (i == result.Length - 1)
                return result;
        }
        while (true)
        {
            for (int i = 0; i < result.Length; i++)
            {
                resultTemp[i] = result[i];
                result[i] = E(time[i], result[i]);
                if (Math.Abs(resultTemp[i] - result[i]) > epsilon)
                    break;
                else if (i == result.Length - 1)
                    return result;
            }
        }
    }
}
