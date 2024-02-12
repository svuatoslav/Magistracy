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
public sealed class ClassicMethodSuccessiveApproximations : SolveKeplerEquation
{
    public ClassicMethodSuccessiveApproximations(double n, double e) : base(n, e) { }
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
                break;
            case 1:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E1(time[i]);
                break;
            case 2:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E2(time[i]);
                break;
            case 3:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E3(time[i]);
                break;
            case 4:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E4(time[i]);
                break;
            case 5:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E5(time[i]);
                break;
            case 6:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E6(time[i]);
                break;
            case 7:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E7(time[i]);
                break;
            case 8:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E8(time[i]);
                break;
            case 9:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E9(time[i]);
                break;
            case 10:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E10(time[i]);
                break;
            default:
                for (int i = 0; i < result.Length; i++)
                    result[i] = E(time[i], 0);
                break;
        }
        //for (int i = 0; i < result.Length; i++)
        //{
        //    result[i] = NumberApproximation switch
        //    {
        //        0 => E0(timeEnd[i]),
        //        1 => E1(timeEnd[i]),
        //        2 => E2(timeEnd[i]),
        //        3 => E3(timeEnd[i]),
        //        4 => E4(timeEnd[i]),
        //        5 => E5(timeEnd[i]),
        //        6 => E6(timeEnd[i]),
        //        7 => E7(timeEnd[i]),
        //        8 => E8(timeEnd[i]),
        //        9 => E9(timeEnd[i]),
        //        10 => E10(timeEnd[i]),
        //        _ => E10(timeEnd[i])
        //    };
        //}
        return result;
    }
    public override double E(double t, double En) => AlgebraicSum(t) / Pow(2, NumberApproximation);
    public override double E0(double t) => M(t);
    public override double E1(double t) => Sin(M(t));
    public double E2(double t) => Sin(2 * M(t)) / 2;
    public double E3(double t) => (Pow(3, 2) * Sin(3 * M(t)) - 3 * Sin(M(t))) / (Pow(2, 3) * Factorial(3));
    public double E4(double t) => (Pow(4, 3) * Sin(4 * M(t)) - 4 * Pow(2, 3) * Sin(2 * M(t))) / (Pow(2, 3) * Factorial(4));
    public double E5(double t) => (Pow(5, 4) * Sin(5 * M(t)) - 5 * Pow(3, 4) * Sin(3 * M(t)) + 10 * Sin(M(t))) / (Pow(2, 4) * Factorial(5));
    public double E6(double t) => (Pow(6, 5) * Sin(6 * M(t)) - 6 * Pow(4, 5) * Sin(4 * M(t)) + 15 * Pow(2, 5) * Sin(2 * M(t))) / (Pow(2, 5) * Factorial(6));
    public double E7(double t) => (Pow(7, 6) * Sin(7 * M(t)) - 7 * Pow(5, 6) * Sin(5 * M(t)) + 21 * Pow(3, 6) * Sin(3 * M(t)) - 35 * Sin(M(t))) / (Pow(2, 6) * Factorial(7));
    public double E8(double t) => (Pow(8, 7) * Sin(8 * M(t)) - 8 * Pow(6, 7) * Sin(6 * M(t)) + 28 * Pow(4, 7) * Sin(4 * M(t)) - 56 * Pow(2, 7) * Sin(2 * M(t)))
        / (Pow(2, 7) * Factorial(8));
    public double E9(double t) => (Pow(9, 8) / Factorial(9) * Sin(9 * M(t)) - Pow(7, 8) / Factorial(8) * Sin(7 * M(t)) + Pow(5, 8) / (2 * Factorial(8)) * Sin(5 * M(t))
        - Pow(3, 8) / (6 * Factorial(6)) * Sin(3 * M(t)) + Sin(M(t)) / Factorial(4) / Factorial(5)) / Pow(2, 8);
    public double E10(double t) => (Pow(10, 9) / Factorial(10) * Sin(9 * M(t)) - Pow(8, 9) / Factorial(9) * Sin(8 * M(t)) + Pow(6, 9) / (2 * Factorial(8)) * Sin(6 * M(t))
        - Pow(4, 9) / (6 * Factorial(7)) * Sin(4 * M(t)) + Pow(2, 9) / (Factorial(4) * Factorial(6)) * Sin(2 * M(t))) / Pow(2, 9);
    private double AlgebraicSum(double t)
    {
        double sum = 0;
        for (int s = 0; s <= NumberApproximation / 2; s++)
        {
            if (s % 2 == 0)
                sum += Pow(NumberApproximation - 2 * s, NumberApproximation - 1) / Factorial(s) / Factorial(NumberApproximation - s) * Sin((NumberApproximation - 2 * s) * M(t));
            else
                sum -= Pow(NumberApproximation - 2 * s, NumberApproximation - 1) / Factorial(s) / Factorial(NumberApproximation - s) * Sin((NumberApproximation - 2 * s) * M(t));
        }
        NumberApproximation++;
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
    public override double E(double t, double En) => En - Pow(M(t) + e * Sin(En) - En, 2) / (En - 2 * (M(t) + e * Sin(En)) + M(t) + e * Sin(M(t) + e * Sin(En)));
    public override double E0(double t) => M(t) + 0.85 * e;
    public override double E1(double t) => M(t) + e * (0.85 - Pow(Sin(M(t) + 0.85 * e) - 0.85, 2) / (0.85 + 2 * Sin(M(t) + 0.85 * e) + Sin(M(t) + e * Sin(M(t) + 0.85 * e))));
    public double E2(double t) => E1(t) - Pow(M(t) + e * Sin(E1(t)) - E1(t), 2) / (E1(t) - 2 * (M(t) + e * Sin(E1(t))) + M(t) + e * Sin(M(t) + e * Sin(E1(t))));
    public double E3(double t) => E2(t) - Pow(M(t) + e * Sin(E2(t)) - E2(t), 2) / (E2(t) - 2 * (M(t) + e * Sin(E2(t))) + M(t) + e * Sin(M(t) + e * Sin(E2(t))));
}

