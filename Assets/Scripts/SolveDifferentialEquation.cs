using System;
using System.Collections.Generic;
using System.Linq;
using DATA;
using UnityEngine;
using static Run;

public static class SolveDifferentialEquation
{
    delegate (EulerAngles, DimensionlessPulses) RK(in (EulerAngles, DimensionlessPulses) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, in double t, in double delta_t, in (double[,] A, double[,] B, double[] C) Butcher);
    delegate (EulerAngles, DimensionlessPulses) RK_adapt(in (EulerAngles, DimensionlessPulses) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, in double t, ref double delta_t, in (double[,] A, double[,] B, double[] C) Butcher, ref double nu_now, in double epsilon);
    public static (EulerAngles, DimensionlessPulses)[] RKCalculate(in (EulerAngles, DimensionlessPulses) initialValues, double[] nu, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, in ODEMethod oDEMethod, in double delta_nu)
    {
        (double[,] A, double[,] B, double[] C) Butcher;
        RK rk = null;
        if (oDEMethod == ODEMethod.RungeKutta_Claccic)
        {
            rk = RK4;
            Butcher = (new double[3, 3] {
                { 1d / 2, 0, 0 },
                { 0, 1d / 2, 0 },
                { 0, 0, 1 } },
                new double[1, 4] { { 1d / 6, 1d / 3, 1d / 3, 1d / 6 } },
                new double[4] { 0, 1d / 2, 1d / 2, 1 });
        }
        else 
        {
            rk = RK4;
            Butcher = (new double[3, 3] {
                { 1d / 3, 0, 0 },
                { -1d / 3, 1, 0 },
                { 1, -1, 1 } },
                new double[1, 4] { { 1d / 8, 3d / 8, 3d / 8, 1d / 8 } },
                new double[4] { 0, 1d / 3, 2d / 3, 1 });
        }
        (EulerAngles, DimensionlessPulses)[] result = new (EulerAngles, DimensionlessPulses)[nu.Length];
        result[0] = initialValues;
        for (int i = 1; i < result.Length; i++)
        {
            if (nu[i] - nu[i - 1] == delta_nu)
                result[i] = rk(result[i - 1], ODEMotions, nu[i - 1], delta_nu, Butcher);
            else if (nu[i] - nu[i - 1] < delta_nu)
                result[i] = rk(result[i - 1], ODEMotions, nu[i - 1], nu[i] - nu[i - 1], Butcher);
            else if (nu[i] - nu[i - 1] > delta_nu)
            {
                (EulerAngles, DimensionlessPulses) temp = result[i - 1];
                for (int j = 0; j < (nu[i] - nu[i - 1]) / delta_nu; j++)
                    temp = rk(temp, ODEMotions, nu[i - 1] + delta_nu * j, delta_nu, Butcher);//(20-10)/0.5=20//(11.2-10/0.5)=2+0.2
                if ((nu[i] - nu[i - 1]) % delta_nu != 0)
                    result[i] = rk(temp, ODEMotions, nu[i - 1] + (nu[i] - nu[i - 1]) / delta_nu, (nu[i] - nu[i - 1]) % delta_nu, Butcher);
                else
                    result[i] = temp;
            }
        }
        return result;
    }
    public static (EulerAngles, DimensionlessPulses)[] RKCalculate(in (EulerAngles, DimensionlessPulses) initialValues, double[] nu, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, in ODEMethod oDEMethod, double delta_nu, in double epsilon)
    {
        (double[,] A, double[,] B, double[] C) Butcher;
        RK_adapt rk_adapt = null;

        if (oDEMethod == ODEMethod.RungeKutta_DormandPrince_45)
        {
            rk_adapt = RK45;
            Butcher = (new double[5, 5] {
            { 1d / 4, 0, 0, 0, 0 },
            { 3d / 32, 9d / 32, 0, 0, 0 },
            { 1932d / 2197, -7200d / 2197, 7296d / 2197, 0, 0 },
            { 439d / 216, -8, 3680d / 513, -845d / 4104, 0 },
            { -8d / 27, 2, -3544d / 2565, 1859d / 4104, -11d / 40 } },
        new double[2, 6] { { 16d / 135, 0, 6656d / 12825, 28561d / 56430, -9d / 50, 2d / 55 },
                            { 25d / 216, 0, 1408d / 2565, 2197d / 4104, -1d / 5, 0 } },
        new double[6] { 0, 1d / 4, 3d / 8, 12d / 13, 1, 1d / 2 });
        }
        else
        {
            rk_adapt = RK45;
            Butcher = (new double[5, 5] {
            { 1d / 4, 0, 0, 0, 0 },
            { 3d / 32, 9d / 32, 0, 0, 0 },
            { 1932d / 2197, -7200d / 2197, 7296d / 2197, 0, 0 },
            { 439d / 216, -8, 3680d / 513, -845d / 4104, 0 },
            { -8d / 27, 2, -3544d / 2565, 1859d / 4104, -11d / 40 } },
            new double[2, 6] { { 16d / 135, 0, 6656d / 12825, 28561d / 56430, -9d / 50, 2d / 55 },
                            { 25d / 216, 0, 1408d / 2565, 2197d / 4104, -1d / 5, 0 } },
        new double[6] { 0, 1d / 4, 3d / 8, 12d / 13, 1, 1d / 2 });
        }
        (EulerAngles, DimensionlessPulses)[] result = new (EulerAngles, DimensionlessPulses)[nu.Length];
        result[0] = initialValues;
        double nu_now = 0;
        double delta_nu_temp = delta_nu;
        double delta_nu_temp1;
        double nu_now_temp;
        (EulerAngles, DimensionlessPulses) temp;
        for (int i = 1; nu_now < nu[^1];)
        {
            if (delta_nu > nu[i] - nu_now)
            {
                //delta_nu = nu[i] - nu_now;
                //temp = rk_adapt(result[i - 1], ODEMotions, nu_now, ref delta_nu, Butcher, ref nu_now, epsilon);
                //отимизация
                nu_now_temp = nu_now;
                delta_nu_temp = nu[i] - nu_now;
                delta_nu_temp1 = delta_nu_temp;
                temp = rk_adapt(result[i - 1], ODEMotions, nu_now, ref delta_nu_temp, Butcher, ref nu_now, epsilon);
                if (nu_now - nu_now_temp < delta_nu_temp1)
                    delta_nu = delta_nu_temp;
            }
            else
                temp = rk_adapt(result[i - 1], ODEMotions, nu_now, ref delta_nu, Butcher, ref nu_now, epsilon);
            if (nu_now == nu[i])
            {
                result[i] = temp;
                i++;
            }
            else if (nu_now > nu[i])
                Debug.LogError($"ошибка в адаптивном методе {nu_now - nu[i]} при шаге {delta_nu}");
        }
        return result;
    }
    internal static (EulerAngles, DimensionlessPulses) RK4(in (EulerAngles angle, DimensionlessPulses impuls) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, in double t, in double delta_t, in (double[,] A, double[,] B, double[] C) Butcher)
    {
        var k = new double[4, ODEMotions.Count];
        for (int i = 0; i < ODEMotions.Count; i++)
            k[0, i] = ODEMotions[i](t + Butcher.C[0] * delta_t, y);
        for (int i = 0; i < ODEMotions.Count; i++)
            k[1, i] = ODEMotions[i](t + Butcher.C[1] * delta_t,
                (new EulerAngles(y.angle.phi + Butcher.A[0, 0] * k[0, 0] * delta_t,
                y.angle.psi + Butcher.A[0, 0] * k[0, 1] * delta_t,
                y.angle.theta + Butcher.A[0, 0] * k[0, 2] * delta_t),
                new DimensionlessPulses(y.impuls.pphi + Butcher.A[0, 0] * k[0, 3] * delta_t,
                y.impuls.ppsi + Butcher.A[0, 0] * k[0, 4] * delta_t,
                y.impuls.ptheta + Butcher.A[0, 0] * k[0, 5] * delta_t)));
        for (int i = 0; i < ODEMotions.Count; i++)
            k[2, i] = ODEMotions[i](t + Butcher.C[2] * delta_t,
                (new EulerAngles(y.angle.phi + (Butcher.A[1, 0] * k[0, 0] + Butcher.A[1, 1] * k[1, 0]) * delta_t,
                y.angle.psi + (Butcher.A[1, 0] * k[0, 1] + Butcher.A[1, 1] * k[1, 1]) * delta_t,
                y.angle.theta + (Butcher.A[1, 0] * k[0, 2] + Butcher.A[1, 1] * k[1, 2]) * delta_t),
                new DimensionlessPulses(y.impuls.pphi + (Butcher.A[1, 0] * k[0, 3] + Butcher.A[1, 1] * k[1, 3]) * delta_t,
                y.impuls.ppsi + (Butcher.A[1, 0] * k[0, 4] + Butcher.A[1, 1] * k[1, 4]) * delta_t,
                y.impuls.ptheta + (Butcher.A[1, 0] * k[0, 5] + Butcher.A[1, 1] * k[1, 5]) * delta_t)));
        for (int i = 0; i < ODEMotions.Count; i++)
            k[3, i] = ODEMotions[i](t + Butcher.C[3] * delta_t,
                (new EulerAngles(y.angle.phi + (Butcher.A[2, 0] * k[0, 0] + Butcher.A[2, 1] * k[1, 0] + Butcher.A[2, 2] * k[2, 0]) * delta_t,
                y.angle.psi + (Butcher.A[2, 0] * k[0, 1] + Butcher.A[2, 1] * k[1, 1] + Butcher.A[2, 2] * k[2, 1]) * delta_t,
                y.angle.theta + (Butcher.A[2, 0] * k[0, 2] + Butcher.A[2, 1] * k[1, 2] + Butcher.A[2, 2] * k[2, 2]) * delta_t),
                new DimensionlessPulses(y.impuls.pphi + (Butcher.A[2, 0] * k[0, 3] + Butcher.A[2, 1] * k[1, 3] + Butcher.A[2, 2] * k[2, 3]) * delta_t,
                y.impuls.ppsi + (Butcher.A[2, 0] * k[0, 4] + Butcher.A[2, 1] * k[1, 4] + Butcher.A[2, 2] * k[2, 4]) * delta_t,
                y.impuls.ptheta + (Butcher.A[2, 0] * k[0, 5] + Butcher.A[2, 1] * k[1, 5] + Butcher.A[2, 2] * k[2, 5]) * delta_t)));
        return (new EulerAngles(y.angle.phi + delta_t * (Butcher.B[0, 0] * k[0, 0] + Butcher.B[0, 1] * k[1, 0] + Butcher.B[0, 2] * k[2, 0] + Butcher.B[0, 3] * k[3, 0]),
            y.angle.psi + delta_t * (Butcher.B[0, 0] * k[0, 1] + Butcher.B[0, 1] * k[1, 1] + Butcher.B[0, 2] * k[2, 1] + Butcher.B[0, 3] * k[3, 1]),
            y.angle.theta + delta_t * (Butcher.B[0, 0] * k[0, 2] + Butcher.B[0, 1] * k[1, 2] + Butcher.B[0, 2] * k[2, 2] + Butcher.B[0, 3] * k[3, 2])),
            new DimensionlessPulses(y.impuls.pphi + delta_t * (Butcher.B[0, 0] * k[0, 3] + Butcher.B[0, 1] * k[1, 3] + Butcher.B[0, 2] * k[2, 3] + Butcher.B[0, 3] * k[3, 3]),
            y.impuls.ppsi + delta_t * (Butcher.B[0, 0] * k[0, 4] + Butcher.B[0, 1] * k[1, 4] + Butcher.B[0, 2] * k[2, 4] + Butcher.B[0, 3] * k[3, 4]),
            y.impuls.ptheta + delta_t * (Butcher.B[0, 0] * k[0, 5] + Butcher.B[0, 1] * k[1, 5] + Butcher.B[0, 2] * k[2, 5] + Butcher.B[0, 3] * k[3, 5])));
    }
    internal static (EulerAngles, DimensionlessPulses) RK45(in (EulerAngles angle, DimensionlessPulses impuls) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, in double t, ref double delta_t, in (double[,] A, double[,] B, double[] C) Butcher, ref double nu_now, in double epsilon)
    {
        var k = new double[6, ODEMotions.Count];
        double[] TE = new double[ODEMotions.Count];
        while (true)
        {
            for (int i = 0; i < ODEMotions.Count; i++)
                k[0, i] = ODEMotions[i](t + Butcher.C[0] * delta_t, y);
            for (int i = 0; i < ODEMotions.Count; i++)
                k[1, i] = ODEMotions[i](t + Butcher.C[1] * delta_t,
                    (new EulerAngles(y.angle.phi + Butcher.A[0, 0] * k[0, 0] * delta_t,
                    y.angle.psi + Butcher.A[0, 0] * k[0, 1] * delta_t,
                    y.angle.theta + Butcher.A[0, 0] * k[0, 2] * delta_t),
                    new DimensionlessPulses(y.impuls.pphi + Butcher.A[0, 0] * k[0, 3] * delta_t,
                    y.impuls.ppsi + Butcher.A[0, 0] * k[0, 4] * delta_t,
                    y.impuls.ptheta + Butcher.A[0, 0] * k[0, 5] * delta_t)));
            for (int i = 0; i < ODEMotions.Count; i++)
                k[2, i] = ODEMotions[i](t + Butcher.C[2] * delta_t,
                    (new EulerAngles(y.angle.phi + (Butcher.A[1, 0] * k[0, 0] + Butcher.A[1, 1] * k[1, 0]) * delta_t,
                    y.angle.psi + (Butcher.A[1, 0] * k[0, 1] + Butcher.A[1, 1] * k[1, 1]) * delta_t,
                    y.angle.theta + (Butcher.A[1, 0] * k[0, 2] + Butcher.A[1, 1] * k[1, 2]) * delta_t),
                    new DimensionlessPulses(y.impuls.pphi + (Butcher.A[1, 0] * k[0, 3] + Butcher.A[1, 1] * k[1, 3]) * delta_t,
                    y.impuls.ppsi + (Butcher.A[1, 0] * k[0, 4] + Butcher.A[1, 1] * k[1, 4]) * delta_t,
                    y.impuls.ptheta + (Butcher.A[1, 0] * k[0, 5] + Butcher.A[1, 1] * k[1, 5]) * delta_t)));
            for (int i = 0; i < ODEMotions.Count; i++)
                k[3, i] = ODEMotions[i](t + Butcher.C[3] * delta_t,
                    (new EulerAngles(y.angle.phi + (Butcher.A[2, 0] * k[0, 0] + Butcher.A[2, 1] * k[1, 0] + Butcher.A[2, 2] * k[2, 0]) * delta_t,
                    y.angle.psi + (Butcher.A[2, 0] * k[0, 1] + Butcher.A[2, 1] * k[1, 1] + Butcher.A[2, 2] * k[2, 1]) * delta_t,
                    y.angle.theta + (Butcher.A[2, 0] * k[0, 2] + Butcher.A[2, 1] * k[1, 2] + Butcher.A[2, 2] * k[2, 2]) * delta_t),
                    new DimensionlessPulses(y.impuls.pphi + (Butcher.A[2, 0] * k[0, 3] + Butcher.A[2, 1] * k[1, 3] + Butcher.A[2, 2] * k[2, 3]) * delta_t,
                    y.impuls.ppsi + (Butcher.A[2, 0] * k[0, 4] + Butcher.A[2, 1] * k[1, 4] + Butcher.A[2, 2] * k[2, 4]) * delta_t,
                    y.impuls.ptheta + (Butcher.A[2, 0] * k[0, 5] + Butcher.A[2, 1] * k[1, 5] + Butcher.A[2, 2] * k[2, 5]) * delta_t)));

            for (int i = 0; i < ODEMotions.Count; i++)
                k[4, i] = ODEMotions[i](t + Butcher.C[4] * delta_t,
                    (new EulerAngles(y.angle.phi + (Butcher.A[3, 0] * k[0, 0] + Butcher.A[3, 1] * k[1, 0] + Butcher.A[3, 2] * k[2, 0] + Butcher.A[3, 3] * k[3, 0]) * delta_t,
                    y.angle.psi + (Butcher.A[3, 0] * k[0, 1] + Butcher.A[3, 1] * k[1, 1] + Butcher.A[3, 2] * k[2, 1] + Butcher.A[3, 3] * k[3, 1]) * delta_t,
                    y.angle.theta + (Butcher.A[3, 0] * k[0, 2] + Butcher.A[3, 1] * k[1, 2] + Butcher.A[3, 2] * k[2, 2] + Butcher.A[3, 3] * k[3, 2]) * delta_t),
                    new DimensionlessPulses(y.impuls.pphi + (Butcher.A[3, 0] * k[0, 3] + Butcher.A[3, 1] * k[1, 3] + Butcher.A[3, 2] * k[2, 3] + Butcher.A[3, 3] * k[3, 3]) * delta_t,
                    y.impuls.ppsi + (Butcher.A[3, 0] * k[0, 4] + Butcher.A[3, 1] * k[1, 4] + Butcher.A[3, 2] * k[2, 4] + Butcher.A[3, 3] * k[3, 4]) * delta_t,
                    y.impuls.ptheta + (Butcher.A[3, 0] * k[0, 5] + Butcher.A[3, 1] * k[1, 5] + Butcher.A[3, 2] * k[2, 5] + Butcher.A[3, 3] * k[3, 5]) * delta_t)));
            for (int i = 0; i < ODEMotions.Count; i++)
                k[5, i] = ODEMotions[i](t + Butcher.C[5] * delta_t,
                    (new EulerAngles(y.angle.phi + (Butcher.A[4, 0] * k[0, 0] + Butcher.A[4, 1] * k[1, 0] + Butcher.A[4, 2] * k[2, 0] + Butcher.A[4, 3] * k[3, 0] + Butcher.A[4, 4] * k[4, 0]) * delta_t,
                    y.angle.psi + (Butcher.A[4, 0] * k[0, 1] + Butcher.A[4, 1] * k[1, 1] + Butcher.A[4, 2] * k[2, 1] + Butcher.A[4, 3] * k[3, 1] + Butcher.A[4, 4] * k[4, 1]) * delta_t,
                    y.angle.theta + (Butcher.A[4, 0] * k[0, 2] + Butcher.A[4, 1] * k[1, 2] + Butcher.A[4, 2] * k[2, 2] + Butcher.A[4, 3] * k[3, 2] + Butcher.A[4, 4] * k[4, 2]) * delta_t),
                    new DimensionlessPulses(y.impuls.pphi + (Butcher.A[4, 0] * k[0, 3] + Butcher.A[4, 1] * k[1, 3] + Butcher.A[4, 2] * k[2, 3] + Butcher.A[4, 3] * k[3, 3] + Butcher.A[4, 4] * k[4, 3]) * delta_t,
                    y.impuls.ppsi + (Butcher.A[4, 0] * k[0, 4] + Butcher.A[4, 1] * k[1, 4] + Butcher.A[4, 2] * k[2, 4] + Butcher.A[4, 3] * k[3, 4] + Butcher.A[4, 4] * k[4, 4]) * delta_t,
                    y.impuls.ptheta + (Butcher.A[4, 0] * k[0, 5] + Butcher.A[4, 1] * k[1, 5] + Butcher.A[4, 2] * k[2, 5] + Butcher.A[4, 3] * k[3, 5] + Butcher.A[4, 4] * k[4, 5]) * delta_t)));
            var y5 = (angle: new EulerAngles(y.angle.phi + (Butcher.B[0, 0] * k[0, 0] + Butcher.B[0, 1] * k[1, 0] + Butcher.B[0, 2] * k[2, 0] + Butcher.B[0, 3] * k[3, 0] + Butcher.B[0, 4] * k[4, 0] + Butcher.B[0, 5] * k[5, 0]) * delta_t,
                y.angle.psi + (Butcher.B[0, 0] * k[0, 1] + Butcher.B[0, 1] * k[1, 1] + Butcher.B[0, 2] * k[2, 1] + Butcher.B[0, 3] * k[3, 1] + Butcher.B[0, 4] * k[4, 1] + Butcher.B[0, 5] * k[5, 1]) * delta_t,
                y.angle.theta + (Butcher.B[0, 0] * k[0, 2] + Butcher.B[0, 1] * k[1, 2] + Butcher.B[0, 2] * k[2, 2] + Butcher.B[0, 3] * k[3, 2] + Butcher.B[0, 4] * k[4, 2] + Butcher.B[0, 5] * k[5, 2]) * delta_t),
                impuls: new DimensionlessPulses(y.impuls.pphi + (Butcher.B[0, 0] * k[0, 3] + Butcher.B[0, 1] * k[1, 3] + Butcher.B[0, 2] * k[2, 3] + Butcher.B[0, 3] * k[3, 3] + Butcher.B[0, 4] * k[4, 3] + Butcher.B[0, 5] * k[5, 3]) * delta_t,
                y.impuls.ppsi + (Butcher.B[0, 0] * k[0, 4] + Butcher.B[0, 1] * k[1, 4] + Butcher.B[0, 2] * k[2, 4] + Butcher.B[0, 3] * k[3, 4] + Butcher.B[0, 4] * k[4, 4] + Butcher.B[0, 5] * k[5, 4]) * delta_t,
                y.impuls.ptheta + (Butcher.B[0, 0] * k[0, 5] + Butcher.B[0, 1] * k[1, 5] + Butcher.B[0, 2] * k[2, 5] + Butcher.B[0, 3] * k[3, 5] + Butcher.B[0, 4] * k[4, 5] + Butcher.B[0, 5] * k[5, 5]) * delta_t));
            var y4 = (angle: new EulerAngles(y.angle.phi + (Butcher.B[1, 0] * k[0, 0] + Butcher.B[1, 1] * k[1, 0] + Butcher.B[1, 2] * k[2, 0] + Butcher.B[1, 3] * k[3, 0] + Butcher.B[1, 4] * k[4, 0] + Butcher.B[1, 5] * k[5, 0]) * delta_t,
                y.angle.psi + (Butcher.B[1, 0] * k[0, 1] + Butcher.B[1, 1] * k[1, 1] + Butcher.B[1, 2] * k[2, 1] + Butcher.B[1, 3] * k[3, 1] + Butcher.B[1, 4] * k[4, 1] + Butcher.B[1, 5] * k[5, 1]) * delta_t,
                y.angle.theta + (Butcher.B[1, 0] * k[0, 2] + Butcher.B[1, 1] * k[1, 2] + Butcher.B[1, 2] * k[2, 2] + Butcher.B[1, 3] * k[3, 2] + Butcher.B[1, 4] * k[4, 2] + Butcher.B[1, 5] * k[5, 2]) * delta_t),
                impuls: new DimensionlessPulses(y.impuls.pphi + (Butcher.B[1, 0] * k[0, 3] + Butcher.B[1, 1] * k[1, 3] + Butcher.B[1, 2] * k[2, 3] + Butcher.B[1, 3] * k[3, 3] + Butcher.B[1, 4] * k[4, 3] + Butcher.B[1, 5] * k[5, 3]) * delta_t,
                y.impuls.ppsi + (Butcher.B[1, 0] * k[0, 4] + Butcher.B[1, 1] * k[1, 4] + Butcher.B[1, 2] * k[2, 4] + Butcher.B[1, 3] * k[3, 4] + Butcher.B[1, 4] * k[4, 4] + Butcher.B[1, 5] * k[5, 4]) * delta_t,
                y.impuls.ptheta + (Butcher.B[1, 0] * k[0, 5] + Butcher.B[1, 1] * k[1, 5] + Butcher.B[1, 2] * k[2, 5] + Butcher.B[1, 3] * k[3, 5] + Butcher.B[1, 4] * k[4, 5] + Butcher.B[1, 5] * k[5, 5]) * delta_t));
            var err = (angle: y5.angle - y4.angle, impuls: y5.impuls - y4.impuls);
            TE[0] = Math.Abs(err.angle.phi);
            TE[1] = Math.Abs(err.angle.psi);
            TE[2] = Math.Abs(err.angle.theta);
            TE[3] = Math.Abs(err.impuls.pphi);
            TE[4] = Math.Abs(err.impuls.ppsi);
            TE[5] = Math.Abs(err.impuls.ptheta);
            if (ComparisonAccuracy(TE, epsilon))
            {
                nu_now += delta_t;
                delta_t *= 0.9 * Math.Pow(epsilon / TE.Max(), 1d / 5);
                return y5;
            }
            else
                delta_t *= 0.9 * Math.Pow(epsilon / TE.Max(), 1d / 5);
        }
    }
    private static bool ComparisonAccuracy(double[] TE, in double epsilon)
    {
        foreach (var abs_error in TE)
            if(abs_error > epsilon)
                return false;
        return true;
    }
}