using System.Collections;
using System.Collections.Generic;
using static System.Math;
using UnityEngine;
using System.Linq;
using System.Numerics;

public abstract class SolveKeplerEquation
{
    protected readonly double n;
    protected readonly double e;
    protected int NumberApproximation;
    protected double Epsilon;
    protected double t0 = 0;
    public abstract double E(double t, double En);
    public SolveKeplerEquation(double n, double e)
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
            if (Abs(resultTemp[i] - result[i]) > epsilon)
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
                if (Abs(resultTemp[i] - result[i]) > epsilon)
                    break;
                else if (i == result.Length - 1)
                    return result;
            }
        }
    }
}
public sealed class IterationMethod : SolveKeplerEquation
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
public sealed class DecompositionDegreesEccentricity : SolveKeplerEquation
{
    public DecompositionDegreesEccentricity(double n, double e) : base(n, e) { }
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
            case 4:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E4(time[i]);
                return result;
            case 5:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E5(time[i]);
                return result;
            case 6:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E6(time[i]);
                return result;
            case 7:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E7(time[i]);
                return result;
            case 8:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E8(time[i]);
                return result;
            case 9:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E9(time[i]);
                return result;
            case 10:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E10(time[i]);
                return result;
            default:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E10(time[i]);
                int NumberApproximationTemp = NumberApproximation;
                NumberApproximation = 11;
                while (NumberApproximation <= NumberApproximationTemp)
                {
                    for (int i = 0; i < result.Length; i++)
                        result[i] = E(time[i], result[i]);
                    NumberApproximation++;
                }
                return result;
        }
    }
    public override double E(double t, double En) => En + Pow(e, NumberApproximation) * AlgebraicSum(t) / Pow(2, NumberApproximation - 1);
    public override double E0(double t) => M(t);
    public override double E1(double t) => E0(t) + e * Sin(M(t));
    public double E2(double t) => E1(t) + Pow(e, 2) * Sin(2 * M(t)) / 2;
    public double E3(double t) => E2(t) + Pow(e, 3) * (Pow(3, 2) * Sin(3 * M(t)) - 3 * Sin(M(t))) / (Pow(2, 2) * Factorial(3));
    public double E4(double t) => E3(t) + Pow(e, 4) * (Pow(4, 3) * Sin(4 * M(t)) - 4 * Pow(2, 3) * Sin(2 * M(t))) / (Pow(2, 3) * Factorial(4));
    public double E5(double t) => E4(t) + Pow(e, 5) * (Pow(5, 4) * Sin(5 * M(t)) - 5 * Pow(3, 4) * Sin(3 * M(t)) + 10 * Sin(M(t))) / (Pow(2, 4) * Factorial(5));
    public double E6(double t) => E5(t) + Pow(e, 6) * (Pow(6, 5) * Sin(6 * M(t)) - 6 * Pow(4, 5) * Sin(4 * M(t)) + 15 * Pow(2, 5) * Sin(2 * M(t))) / (Pow(2, 5) * Factorial(6));
    public double E7(double t) => E6(t) + Pow(e, 7) * (Pow(7, 6) * Sin(7 * M(t)) - 7 * Pow(5, 6) * Sin(5 * M(t)) + 21 * Pow(3, 6) * Sin(3 * M(t)) - 35 * Sin(M(t))) / (Pow(2, 6) * Factorial(7));
    public double E8(double t) => E7(t) + Pow(e, 8) * (Pow(8, 7) * Sin(8 * M(t)) - 8 * Pow(6, 7) * Sin(6 * M(t)) + 28 * Pow(4, 7) * Sin(4 * M(t)) - 56 * Pow(2, 7) * Sin(2 * M(t)))
        / (Pow(2, 7) * Factorial(8));
    public double E9(double t) => E8(t) + Pow(e, 9) * (Pow(9, 8) / Factorial(9) * Sin(9 * M(t)) - Pow(7, 8) / Factorial(8) * Sin(7 * M(t)) + Pow(5, 8) / (2 * Factorial(8)) * Sin(5 * M(t))
        - Pow(3, 8) / (6 * Factorial(6)) * Sin(3 * M(t)) + Sin(M(t)) / Factorial(4) / Factorial(5)) / Pow(2, 8);
    public double E10(double t) => E9(t) + Pow(e, 10) * (Pow(10, 9) / Factorial(10) * Sin(9 * M(t)) - Pow(8, 9) / Factorial(9) * Sin(8 * M(t)) + Pow(6, 9) / (2 * Factorial(8)) * Sin(6 * M(t))
        - Pow(4, 9) / (6 * Factorial(7)) * Sin(4 * M(t)) + Pow(2, 9) / (Factorial(4) * Factorial(6)) * Sin(2 * M(t))) / Pow(2, 9);
    private double AlgebraicSum(double t)
    {
        double sum = 0;
        for (int s = 0; s <= NumberApproximation / 2; s++)
            sum += Pow(-1, s) * Pow(NumberApproximation - 2 * s, NumberApproximation - 1) / (Factorial(s) * Factorial(NumberApproximation - s)) * Sin((NumberApproximation - 2 * s) * M(t));
        return sum;
    }
}
public sealed class DenbyMethod : SolveKeplerEquation
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

