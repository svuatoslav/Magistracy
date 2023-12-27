using System;
using System.Collections.Generic;
using DATA;

public class SolveDifferentialEquation
{
    private double delta_t;
    private readonly (double[,], double[], double[]) ButcherClassic = (//_A, b, c
        new double[3, 3] { 
            { 1d / 2, 0, 0 }, 
            { 0, 1d / 2, 0 }, 
            { 0, 0, 1 } },
        new double[4] { 0, 1d / 2, 1d / 2, 1 },
        new double[4] { 1d / 6, 1d / 3, 1d / 3, 1d / 6 });
    private readonly (double[,], double[], double[]) Butcher3to8 = (
        new double[3, 3] { 
            { 1d / 3, 0, 0 }, 
            { -1d / 3, 1, 0 }, 
            { 1, -1, 1 } },
        new double[4] { 0, 1d / 3, 2d / 3, 1 },
        new double[4] { 1d / 8, 3d / 8, 3d / 8, 1d / 8 });
    private readonly (double[,], double[,], double[]) ButcherFehlberg45 = (
        new double[5, 5] {
            { 1d / 4, 0, 0, 0, 0 },
            { 3d / 32, 9d / 32, 0, 0, 0 },
            { 1932d / 2197, -7200d / 2197, 7296d / 2197, 0, 0 },
            { 439d / 216, -8, 3680d / 513, -845d / 4104, 0 },
            { -8d / 27, 2, -3544d / 2565, 1859d / 4104, -11d / 40 } },
        new double[2, 6] { { 0, 1d / 4, 3d / 8, 12d / 13, 1, 1d / 2 }, { 16d / 135, 0, 6656d / 12825, 28561d / 56430, -9d / 50, 2d / 55 } },
        new double[6] { 25d / 216, 0, 1408d / 2565, 2197d / 4104, -1d / 5, 0 });
    private readonly double[] ControlMemberFehlberg45 = new double[6] { 1d / 360, 0, -128d / 4275, -2197d / 75240, 1d / 50, 2d / 55 };
    private readonly double[][] Butcher_4thOrder = {
        new double[] { 0 },
        new double[] { 1d / 2, 1d / 2 },
        new double[] { 1d / 2, 0, 1d / 2 },
        new double[] { 1, 0, 0, 1},
        new double[] { 1d / 6, 1d / 3, 1d / 3, 1d / 6}
    };
    private readonly double[][] Butcher_4thOrder3to8 = {
        new double[] { 0 },
        new double[] { 1d / 3, 1d / 3 },
        new double[] { 2d / 3, -1d / 3, 1},
        new double[] { 1, 1, -1, 1},
        new double[] { 1d / 8, 3d / 8, 3d / 8, 1d / 8 }
    };
    private readonly double[][] Butcher_Fehlberg = {
        new double[] { 0 },
        new double[] { 1d / 4, 1d / 4 },
        new double[] { 3d / 8, 3d / 32, 9d / 32},
        new double[] { 12d / 13, 1932d / 2197, -7200d / 2197, 7296d / 2197 },
        new double[] { 1, 439d / 216, -8, 3680d / 513, -845d / 4104 },
        new double[] { 1d / 2, -8d / 27, 2, -3544d / 2565, 1859d / 4104, -11d / 40  },
        new double[] { 16d / 135, 0, 6656d / 12825, 28561d / 56430, -9d / 50, 2d / 55 },
        new double[] { 25d / 216, 0, 1408d / 2565, 2197d / 4104, -1d / 5, 0 },
    };
    public SolveDifferentialEquation(double delta_t)
    {
        this.delta_t = delta_t;
    }
    internal (EulerAngles, DimensionlessPulses) RKClassic((EulerAngles, DimensionlessPulses) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, double t)
    {
        var k = new double[ODEMotions.Count, ODEMotions.Count];
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[0, i] = ODEMotions[i](t + Butcher_4thOrder[0][0] * delta_t, y);
        }
        var y_k1 = (new EulerAngles(y.Item1.phi + Butcher_4thOrder[1][1] * k[0, 0] * delta_t,
            y.Item1.psi + Butcher_4thOrder[1][1] * k[0, 1] * delta_t,
            y.Item1.theta + Butcher_4thOrder[1][1] * k[0, 2] * delta_t), 
            new DimensionlessPulses(y.Item2.pphi + Butcher_4thOrder[1][1] * k[0, 3] * delta_t,
            y.Item2.ppsi + Butcher_4thOrder[1][1] * k[0, 4] * delta_t,
            y.Item2.ptheta + Butcher_4thOrder[1][1] * k[0, 5] * delta_t));
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[1, i] = ODEMotions[i](t + Butcher_4thOrder[1][0] * delta_t, y_k1);
        }
        var y_k2 = (new EulerAngles(y.Item1.phi + (Butcher_4thOrder[2][1] * k[0, 0] + Butcher_4thOrder[2][2] * k[1, 0]) * delta_t,
            y.Item1.psi + (Butcher_4thOrder[2][1] * k[0, 1] + Butcher_4thOrder[2][2] * k[1, 1]) * delta_t,
            y.Item1.theta + (Butcher_4thOrder[2][1] * k[0, 2] + Butcher_4thOrder[2][2] * k[1, 2]) * delta_t),
            new DimensionlessPulses(y.Item2.pphi + (Butcher_4thOrder[2][1] * k[0, 3] + Butcher_4thOrder[2][2] * k[1, 3]) * delta_t,
            y.Item2.ppsi + (Butcher_4thOrder[2][1] * k[0, 4] + Butcher_4thOrder[2][2] * k[1, 4]) * delta_t,
            y.Item2.ptheta + (Butcher_4thOrder[2][1] * k[0, 5] + Butcher_4thOrder[2][2] * k[1, 5]) * delta_t));
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[2, i] = ODEMotions[i](t + Butcher_4thOrder[2][0] * delta_t, y_k2);
        }
        var y_k3 = (new EulerAngles(y.Item1.phi + (Butcher_4thOrder[3][1] * k[0, 0] + Butcher_4thOrder[3][2] * k[1, 0] + Butcher_4thOrder[3][3] * k[2, 0]) * delta_t,
            y.Item1.psi + (Butcher_4thOrder[3][1] * k[0, 1] + Butcher_4thOrder[3][2] * k[1, 1] + Butcher_4thOrder[3][3] * k[2, 1]) * delta_t, 
            y.Item1.theta + (Butcher_4thOrder[3][1] * k[0, 2] + Butcher_4thOrder[3][2] * k[1, 2] + Butcher_4thOrder[3][3] * k[2, 2]) * delta_t),
            new DimensionlessPulses(y.Item2.pphi + (Butcher_4thOrder[3][1] * k[0, 3] + Butcher_4thOrder[3][2] * k[1, 3] + Butcher_4thOrder[3][3] * k[2, 3]) * delta_t, 
            y.Item2.ppsi + (Butcher_4thOrder[3][1] * k[0, 4] + Butcher_4thOrder[3][2] * k[1, 4] + Butcher_4thOrder[3][3] * k[2, 4]) * delta_t, 
            y.Item2.ptheta + (Butcher_4thOrder[3][1] * k[0, 5] + Butcher_4thOrder[3][2] * k[1, 5] + Butcher_4thOrder[3][3] * k[2, 5]) * delta_t));
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[3, i] = ODEMotions[i](t + Butcher_4thOrder[3][0] * delta_t, y_k3);
        }
        return (new EulerAngles(y.Item1.phi + delta_t * (Butcher_4thOrder[4][0] * k[0, 0] + Butcher_4thOrder[4][1] * k[1, 0] + Butcher_4thOrder[4][2] * k[2, 0] + Butcher_4thOrder[4][3] * k[3, 0]), 
            y.Item1.psi + delta_t * (Butcher_4thOrder[4][0] * k[0, 1] + Butcher_4thOrder[4][1] * k[1, 1] + Butcher_4thOrder[4][2] * k[2, 1] + Butcher_4thOrder[4][3] * k[3, 1]), 
            y.Item1.theta + delta_t * (Butcher_4thOrder[4][0] * k[0, 2] + Butcher_4thOrder[4][1] * k[1, 2] + Butcher_4thOrder[4][2] * k[2, 2] + Butcher_4thOrder[4][3] * k[3, 2])),
            new DimensionlessPulses(y.Item2.pphi + delta_t * (Butcher_4thOrder[4][0] * k[0, 3] + Butcher_4thOrder[4][1] * k[1, 3] + Butcher_4thOrder[4][2] * k[2, 3] + Butcher_4thOrder[4][3] * k[3, 3]),
            y.Item2.ppsi + delta_t * (Butcher_4thOrder[4][0] * k[0, 4] + Butcher_4thOrder[4][1] * k[1, 4] + Butcher_4thOrder[4][2] * k[2, 4] + Butcher_4thOrder[4][3] * k[3, 4]), 
            y.Item2.ptheta + delta_t * (Butcher_4thOrder[4][0] * k[0, 5] + Butcher_4thOrder[4][1] * k[1, 5] + Butcher_4thOrder[4][2] * k[2, 5] + Butcher_4thOrder[4][3] * k[3, 5])));
    }
    internal (EulerAngles, DimensionlessPulses) RK3to8((EulerAngles, DimensionlessPulses) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, double t)
    {
        var k = new double[4, ODEMotions.Count];
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[0, i] = ODEMotions[i](t + Butcher_4thOrder3to8[0][0] * delta_t, y);
        }
        var y_k1 = (new EulerAngles(y.Item1.phi + Butcher_4thOrder3to8[1][1] * k[0, 0] * delta_t,
            y.Item1.psi + Butcher_4thOrder3to8[1][1] * k[0, 1] * delta_t,
            y.Item1.theta + Butcher_4thOrder3to8[1][1] * k[0, 2] * delta_t),
            new DimensionlessPulses(y.Item2.pphi + Butcher_4thOrder3to8[1][1] * k[0, 3] * delta_t, 
            y.Item2.ppsi + Butcher_4thOrder3to8[1][1] * k[0, 4] * delta_t, 
            y.Item2.ptheta + Butcher_4thOrder3to8[1][1] * k[0, 5] * delta_t));
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[1, i] = ODEMotions[i](t + Butcher_4thOrder3to8[1][0] * delta_t, y_k1);
        }
        var y_k2 = (new EulerAngles(y.Item1.phi + (Butcher_4thOrder3to8[2][1] * k[0, 0] + Butcher_4thOrder3to8[2][2] * k[1, 0]) * delta_t,
            y.Item1.psi + (Butcher_4thOrder3to8[2][1] * k[0, 1] + Butcher_4thOrder3to8[2][2] * k[1, 1]) * delta_t,
            y.Item1.theta + (Butcher_4thOrder3to8[2][1] * k[0, 2] + Butcher_4thOrder3to8[2][2] * k[1, 2]) * delta_t),
            new DimensionlessPulses(y.Item2.pphi + (Butcher_4thOrder3to8[2][1] * k[0, 3] + Butcher_4thOrder3to8[2][2] * k[1, 3]) * delta_t,
            y.Item2.ppsi + (Butcher_4thOrder3to8[2][1] * k[0, 4] + Butcher_4thOrder3to8[2][2] * k[1, 4]) * delta_t,
            y.Item2.ptheta + (Butcher_4thOrder3to8[2][1] * k[0, 5] + Butcher_4thOrder3to8[2][2] * k[1, 5]) * delta_t));
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[2, i] = ODEMotions[i](t + Butcher_4thOrder3to8[2][0] * delta_t, y_k2);
        }
        var y_k3 = (new EulerAngles(y.Item1.phi + (Butcher_4thOrder3to8[3][1] * k[0, 0] + Butcher_4thOrder3to8[3][2] * k[1, 0] + Butcher_4thOrder3to8[3][3] * k[2, 0]) * delta_t,
            y.Item1.psi + (Butcher_4thOrder3to8[3][1] * k[0, 1] + Butcher_4thOrder3to8[3][2] * k[1, 1] + Butcher_4thOrder3to8[3][3] * k[2, 1]) * delta_t,
            y.Item1.theta + (Butcher_4thOrder3to8[3][1] * k[0, 2] + Butcher_4thOrder3to8[3][2] * k[1, 2] + Butcher_4thOrder3to8[3][3] * k[2, 2]) * delta_t),
            new DimensionlessPulses(y.Item2.pphi + (Butcher_4thOrder3to8[3][1] * k[0, 3] + Butcher_4thOrder3to8[3][2] * k[1, 3] + Butcher_4thOrder3to8[3][3] * k[2, 3]) * delta_t,
            y.Item2.ppsi + (Butcher_4thOrder3to8[3][1] * k[0, 4] + Butcher_4thOrder3to8[3][2] * k[1, 4] + Butcher_4thOrder3to8[3][3] * k[2, 4]) * delta_t,
            y.Item2.ptheta + (Butcher_4thOrder3to8[3][1] * k[0, 5] + Butcher_4thOrder3to8[3][2] * k[1, 5] + Butcher_4thOrder3to8[3][3] * k[2, 5]) * delta_t));
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[3, i] = ODEMotions[i](t + Butcher_4thOrder3to8[3][0] * delta_t, y_k3);
        }
        return (new EulerAngles(y.Item1.phi + delta_t * (Butcher_4thOrder3to8[4][0] * k[0, 0] + Butcher_4thOrder3to8[4][1] * k[1, 0] + Butcher_4thOrder3to8[4][2] * k[2, 0] + Butcher_4thOrder3to8[4][3] * k[3, 0]),
            y.Item1.psi + delta_t * (Butcher_4thOrder3to8[4][0] * k[0, 1] + Butcher_4thOrder3to8[4][1] * k[1, 1] + Butcher_4thOrder3to8[4][2] * k[2, 1] + Butcher_4thOrder3to8[4][3] * k[3, 1]),
            y.Item1.theta + delta_t * (Butcher_4thOrder3to8[4][0] * k[0, 2] + Butcher_4thOrder3to8[4][1] * k[1, 2] + Butcher_4thOrder3to8[4][2] * k[2, 2] + Butcher_4thOrder3to8[4][3] * k[3, 2])),
            new DimensionlessPulses(y.Item2.pphi + delta_t * (Butcher_4thOrder3to8[4][0] * k[0, 3] + Butcher_4thOrder3to8[4][1] * k[1, 3] + Butcher_4thOrder3to8[4][2] * k[2, 3] + Butcher_4thOrder3to8[4][3] * k[3, 3]),
            y.Item2.ppsi + delta_t * (Butcher_4thOrder3to8[4][0] * k[0, 4] + Butcher_4thOrder3to8[4][1] * k[1, 4] + Butcher_4thOrder3to8[4][2] * k[2, 4] + Butcher_4thOrder3to8[4][3] * k[3, 4]),
            y.Item2.ptheta + delta_t * (Butcher_4thOrder3to8[4][0] * k[0, 5] + Butcher_4thOrder3to8[4][1] * k[1, 5] + Butcher_4thOrder3to8[4][2] * k[2, 5] + Butcher_4thOrder3to8[4][3] * k[3, 5])));
    }
    internal (EulerAngles, DimensionlessPulses) RKFehlberg((EulerAngles, DimensionlessPulses) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, double t, double delta_t)
    {
        var k = new double[6, ODEMotions.Count];
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[0, i] = ODEMotions[i](t + Butcher_Fehlberg[0][0] * delta_t, y);
        }
        var y_k1 = (new EulerAngles(y.Item1.phi + Butcher_Fehlberg[1][1] * k[0, 0] * delta_t,
            y.Item1.psi + Butcher_Fehlberg[1][1] * k[0, 1] * delta_t,
            y.Item1.theta + Butcher_Fehlberg[1][1] * k[0, 2] * delta_t),
            new DimensionlessPulses(y.Item2.pphi + Butcher_Fehlberg[1][1] * k[0, 3] * delta_t, 
            y.Item2.ppsi + Butcher_Fehlberg[1][1] * k[0, 4] * delta_t, 
            y.Item2.ptheta + Butcher_Fehlberg[1][1] * k[0, 5] * delta_t));
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[1, i] = ODEMotions[i](t + Butcher_Fehlberg[1][0] * delta_t, y_k1);
        }
        var y_k2 = (new EulerAngles(y.Item1.phi + (Butcher_Fehlberg[2][1] * k[0, 0] + Butcher_Fehlberg[2][2] * k[1, 0]) * delta_t,
            y.Item1.psi + (Butcher_Fehlberg[2][1] * k[0, 1] + Butcher_Fehlberg[2][2] * k[1, 1]) * delta_t,
            y.Item1.theta + (Butcher_Fehlberg[2][1] * k[0, 2] + Butcher_Fehlberg[2][2] * k[1, 2]) * delta_t),
            new DimensionlessPulses(y.Item2.pphi + (Butcher_Fehlberg[2][1] * k[0, 3] + Butcher_Fehlberg[2][2] * k[1, 3]) * delta_t,
            y.Item2.ppsi + (Butcher_Fehlberg[2][1] * k[0, 4] + Butcher_Fehlberg[2][2] * k[1, 4]) * delta_t,
            y.Item2.ptheta + (Butcher_Fehlberg[2][1] * k[0, 5] + Butcher_Fehlberg[2][2] * k[1, 5]) * delta_t));
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[2, i] = ODEMotions[i](t + Butcher_Fehlberg[2][0] * delta_t, y_k2);
        }
        var y_k3 = (new EulerAngles(y.Item1.phi + (Butcher_Fehlberg[3][1] * k[0, 0] + Butcher_Fehlberg[3][2] * k[1, 0] + Butcher_Fehlberg[3][3] * k[2, 0]) * delta_t,
            y.Item1.psi + (Butcher_Fehlberg[3][1] * k[0, 1] + Butcher_Fehlberg[3][2] * k[1, 1] + Butcher_Fehlberg[3][3] * k[2, 1]) * delta_t,
            y.Item1.theta + (Butcher_Fehlberg[3][1] * k[0, 2] + Butcher_Fehlberg[3][2] * k[1, 2] + Butcher_Fehlberg[3][3] * k[2, 2]) * delta_t),
            new DimensionlessPulses(y.Item2.pphi + (Butcher_Fehlberg[3][1] * k[0, 3] + Butcher_Fehlberg[3][2] * k[1, 3] + Butcher_Fehlberg[3][3] * k[2, 3]) * delta_t,
            y.Item2.ppsi + (Butcher_Fehlberg[3][1] * k[0, 4] + Butcher_Fehlberg[3][2] * k[1, 4] + Butcher_Fehlberg[3][3] * k[2, 4]) * delta_t,
            y.Item2.ptheta + (Butcher_Fehlberg[3][1] * k[0, 5] + Butcher_Fehlberg[3][2] * k[1, 5] + Butcher_Fehlberg[3][3] * k[2, 5]) * delta_t));
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[3, i] = ODEMotions[i](t + Butcher_Fehlberg[3][0] * delta_t, y_k3);
        }
        var y_k4 = (new EulerAngles(y.Item1.phi + (Butcher_Fehlberg[4][1] * k[0, 0] + Butcher_Fehlberg[4][2] * k[1, 0] + Butcher_Fehlberg[4][3] * k[2, 0] + Butcher_Fehlberg[4][4] * k[3, 0]) * delta_t,
            y.Item1.psi + (Butcher_Fehlberg[4][1] * k[0, 1] + Butcher_Fehlberg[4][2] * k[1, 1] + Butcher_Fehlberg[4][3] * k[2, 1] + Butcher_Fehlberg[4][4] * k[3, 1]) * delta_t,
            y.Item1.theta + (Butcher_Fehlberg[4][1] * k[0, 2] + Butcher_Fehlberg[4][2] * k[1, 2] + Butcher_Fehlberg[4][3] * k[2, 2] + Butcher_Fehlberg[4][4] * k[3, 2]) * delta_t),
            new DimensionlessPulses(y.Item2.pphi + (Butcher_Fehlberg[4][1] * k[0, 3] + Butcher_Fehlberg[4][2] * k[1, 3] + Butcher_Fehlberg[4][3] * k[2, 3] + Butcher_Fehlberg[4][4] * k[3, 3]) * delta_t,
            y.Item2.ppsi + (Butcher_Fehlberg[4][1] * k[0, 4] + Butcher_Fehlberg[4][2] * k[1, 4] + Butcher_Fehlberg[4][3] * k[2, 4] + Butcher_Fehlberg[4][4] * k[3, 4]) * delta_t,
            y.Item2.ptheta + (Butcher_Fehlberg[4][1] * k[0, 5] + Butcher_Fehlberg[4][2] * k[1, 5] + Butcher_Fehlberg[4][3] * k[2, 5] + Butcher_Fehlberg[4][4] * k[3, 5]) * delta_t));
        
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[4, i] = ODEMotions[i](t + Butcher_Fehlberg[4][0] * delta_t, y_k4);
        }
        var y_k5 = (new EulerAngles(y.Item1.phi + (Butcher_Fehlberg[5][1] * k[0, 0] + Butcher_Fehlberg[5][2] * k[1, 0] + Butcher_Fehlberg[5][3] * k[2, 0] + Butcher_Fehlberg[5][4] * k[3, 0] + Butcher_Fehlberg[5][5] * k[4, 0]) * delta_t,
            y.Item1.psi + (Butcher_Fehlberg[5][1] * k[0, 1] + Butcher_Fehlberg[5][2] * k[1, 1] + Butcher_Fehlberg[5][3] * k[2, 1] + Butcher_Fehlberg[5][4] * k[3, 1] + Butcher_Fehlberg[5][5] * k[4, 1]) * delta_t,
            y.Item1.theta + (Butcher_Fehlberg[5][1] * k[0, 2] + Butcher_Fehlberg[5][2] * k[1, 2] + Butcher_Fehlberg[5][3] * k[2, 2] + Butcher_Fehlberg[5][4] * k[3, 2] + Butcher_Fehlberg[5][5] * k[4, 2]) * delta_t),
            new DimensionlessPulses(y.Item2.pphi + (Butcher_Fehlberg[5][1] * k[0, 3] + Butcher_Fehlberg[5][2] * k[1, 3] + Butcher_Fehlberg[5][3] * k[2, 3] + Butcher_Fehlberg[5][4] * k[3, 3] + Butcher_Fehlberg[5][5] * k[4, 3]) * delta_t,
            y.Item2.ppsi + (Butcher_Fehlberg[5][1] * k[0, 4] + Butcher_Fehlberg[5][2] * k[1, 4] + Butcher_Fehlberg[5][3] * k[2, 4] + Butcher_Fehlberg[5][4] * k[3, 4] + Butcher_Fehlberg[5][5] * k[4, 4]) * delta_t,
            y.Item2.ptheta + (Butcher_Fehlberg[5][1] * k[0, 5] + Butcher_Fehlberg[5][2] * k[1, 5] + Butcher_Fehlberg[5][3] * k[2, 5] + Butcher_Fehlberg[5][4] * k[3, 5] + Butcher_Fehlberg[5][5] * k[4, 5]) * delta_t));
        for (int i = 0; i < ODEMotions.Count; i++)
        {
            k[5, i] = ODEMotions[i](t + Butcher_Fehlberg[5][0] * delta_t, y_k5);
        }
        return (new EulerAngles(y.Item1.phi + (Butcher_Fehlberg[6][0] * k[0, 0] + Butcher_Fehlberg[6][1] * k[1, 0] + Butcher_Fehlberg[6][2] * k[2, 0] + Butcher_Fehlberg[6][3] * k[3, 0] + Butcher_Fehlberg[6][4] * k[4, 0] + Butcher_Fehlberg[6][5] * k[5, 0]) * delta_t,
            y.Item1.psi + (Butcher_Fehlberg[6][0] * k[0, 1] + Butcher_Fehlberg[6][1] * k[1, 1] + Butcher_Fehlberg[6][2] * k[2, 1] + Butcher_Fehlberg[6][3] * k[3, 1] + Butcher_Fehlberg[6][4] * k[4, 1] + Butcher_Fehlberg[6][5] * k[5, 1]) * delta_t,
            y.Item1.theta + (Butcher_Fehlberg[6][0] * k[0, 2] + Butcher_Fehlberg[6][1] * k[1, 2] + Butcher_Fehlberg[6][2] * k[2, 2] + Butcher_Fehlberg[6][3] * k[3, 2] + Butcher_Fehlberg[6][4] * k[4, 2] + Butcher_Fehlberg[6][5] * k[5, 2]) * delta_t),
            new DimensionlessPulses(y.Item2.pphi + (Butcher_Fehlberg[6][0] * k[0, 3] + Butcher_Fehlberg[6][1] * k[1, 3] + Butcher_Fehlberg[6][2] * k[2, 3] + Butcher_Fehlberg[6][3] * k[3, 3] + Butcher_Fehlberg[6][4] * k[4, 3] + Butcher_Fehlberg[6][5] * k[5, 3]) * delta_t,
            y.Item2.ppsi + (Butcher_Fehlberg[6][0] * k[0, 4] + Butcher_Fehlberg[6][1] * k[1, 4] + Butcher_Fehlberg[6][2] * k[2, 4] + Butcher_Fehlberg[6][3] * k[3, 4] + Butcher_Fehlberg[6][4] * k[4, 4] + Butcher_Fehlberg[6][5] * k[5, 4]) * delta_t,
            y.Item2.ptheta + (Butcher_Fehlberg[6][0] * k[0, 5] + Butcher_Fehlberg[6][1] * k[1, 5] + Butcher_Fehlberg[6][2] * k[2, 5] + Butcher_Fehlberg[6][3] * k[3, 5] + Butcher_Fehlberg[6][4] * k[4, 5] + Butcher_Fehlberg[6][5] * k[5, 5]) * delta_t));
    }
}
