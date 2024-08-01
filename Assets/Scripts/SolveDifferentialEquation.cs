using System;
using System.Collections.Generic;
using System.Linq;
using DATA;

public static class SolveDifferentialEquation
{
    delegate (EulerAngles, DimensionlessPulses) RK(in (EulerAngles, DimensionlessPulses) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions,
        in double t, in double delta_t, in (double[,] A, double[,] B, double[] C) Butcher);
    delegate (EulerAngles, DimensionlessPulses) RK_adapt(in (EulerAngles, DimensionlessPulses) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions,
        in ODEMethod odeMethod, ref double t, ref double delta_t, in (double[,] A, double[,] B, double[] C, double[] D) Butcher, in double epsilon);
    public static (EulerAngles, DimensionlessPulses)[] RKCalculate(in (EulerAngles, DimensionlessPulses) initialValues, double[] nu, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, in ODEMethod oDEMethod, in double delta_nu)
    {
        (double[,] A, double[,] B, double[] C) Butcher;
        RK rk = RK4;
        if (oDEMethod == ODEMethod.RungeKutta_Claccic) 
            Butcher = (new double[3, 3] {
                    { 1d / 2, 0, 0 },
                    { 0, 1d / 2, 0 },
                    { 0, 0, 1 } },
                new double[1, 4] { { 1d / 6, 1d / 3, 1d / 3, 1d / 6 } },
                new double[4] { 0, 1d / 2, 1d / 2, 1 });
        else //oDEMethod == ODEMethod.RungeKutta_3_8
            Butcher = (new double[3, 3] {
                    { 1d / 3, 0, 0 },
                    { -1d / 3, 1, 0 },
                    { 1, -1, 1 } },
                new double[1, 4] { { 1d / 8, 3d / 8, 3d / 8, 1d / 8 } },
                new double[4] { 0, 1d / 3, 2d / 3, 1 });
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
    public static (EulerAngles, DimensionlessPulses)[] RKCalculate(in (EulerAngles, DimensionlessPulses) initialValues, double[] nu, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, in ODEMethod odeMethod, double delta_nu, in double epsilon)
    {
        (double[,] A, double[,] B, double[] C, double[] D) Butcher;
        RK_adapt rk_adapt = RK_adapt_step;
        if (odeMethod == ODEMethod.RungeKutta_DormandPrince_78)
            Butcher = (new double[12, 12] {
                    { 1d / 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 1d / 48, 1d / 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 1d / 32, 0, 3d / 32, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 5d / 16, 0, -75d / 64, 75d / 64, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 3d / 80, 0, 0, 3d / 16, 3d / 20, 0, 0, 0, 0, 0, 0, 0 },
                    { 29443841d / 614563906, 0, 0, 77736538d / 692538347, -28693883d / 1125000000, 23124283d / 1800000000, 0, 0, 0, 0, 0, 0 },
                    { 16016141d / 946692911, 0, 0, 61564180d / 158732637, 22789713d / 633445777, 545815736d / 2771057229, -180193667d / 1043307555, 0, 0, 0, 0, 0 },
                    { 39632708d / 573591083, 0, 0, -433636366d / 683701615, -421739975d / 2616292301, 100302831d / 723423059, 790204164d / 839813087, 800635310d / 3783071287, 0, 0, 0, 0 },
                    { 246121993d / 1340847787, 0, 0, -37695042795d / 15268766246, -309121744d / 1061227803, -12992083d / 490766935, 6005943493d / 2108947869, 393006217d / 1396673457, 123872331d / 1001029789, 0, 0, 0 },
                    { -1028468189d / 846180014, 0, 0, 8478235783d / 508512852, 1311729495d / 1432422823, -10304129995d / 1701304382, -48777925059d / 3047939560, 15336726248d / 1032824649, -45442868181d / 3398467696, 3065993473d / 597172653, 0, 0 },
                    { 185892177d / 718116043, 0, 0, -3185094517 / 667107341, -477755414d / 1098053517, -703635378d / 230739211, 5731566787d / 1027545527, 5232866602d / 850066563, -4093664535d / 808688257, 3962137247d / 1805957418, 65686358d / 487910083, 0 },
                    { 403863854d / 491063109, 0, 0, -5068492393d / 434740067, -411421997d / 543043805, 652783627d / 914296604, 11173962825d / 925320556, -13158990841d / 6184727034, 3936647629d / 1978049680, -160528059d / 685178525, 248638103d / 1413531060, 0 }, },
                new double[2, 13] {
                    { 13451932d / 455176623, 0, 0, 0, 0, -808719846d / 976000145, 1757004468d / 5645159321, 656045339d / 265891186, -3867574721d / 1518517206, 465885868d / 322736535,  53011238d / 667516719, 2d / 45, 0 },
                    { 14005451d / 335480064, 0, 0, 0, 0, -59238493d / 1068277825, 181606767d / 758867731, 561292985d / 797845732, -1041891430d / 1371343529, 760417239d / 1151165299,  118820643d / 751138087, -528747749d / 2220607170, 1d / 4 }},
                new double[13] { 0, 1d / 18, 1d / 12, 1d / 8, 5d / 16, 3d / 8, 59d / 400, 93d / 200, 5490023248d / 9719169821, 13d / 20, 1201146811d / 1299019798, 1, 1 },
                new double[13] { -206899875720925d / 16966964735038208, 0, 0, 0, 0, -161224140072326693d / 208527862420056925, 308134860501296901d / 4283929245060770651, 187090058122256469d / 106070073963259076, -3721643503328385829d / 2082408744123259974, 290897219666967667d / 371523099811498965, -39496005864008611d / 501397231350176553, 627441401d / 2220607170, -1d / 4 });
        else if (odeMethod == ODEMethod.RungeKutta_Fehlberg_78)
            Butcher = (new double[12, 12] {
                    { 2d / 27, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 1d / 36, 1d / 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 1d / 24, 0, 1d / 8, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 5d / 12, 0, -25d / 16, 25d / 16, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 1d / 20, 0, 0, 1d / 4, 1d / 5, 0, 0, 0, 0, 0, 0, 0 },
                    { -25d / 108, 0, 0, 125d / 108, -65d / 27, 125d / 54, 0, 0, 0, 0, 0, 0 },
                    { 31d / 300, 0, 0, 0, 61d / 225, -2d / 9, 13d / 900, 0, 0, 0, 0, 0 },
                    { 2, 0, 0, -53d / 6, 704d / 45, -107d / 9, 67d / 90, 3, 0, 0, 0, 0 },
                    { -91d / 108, 0, 0, 23d / 108, -976d / 135, 311d / 54, -19d / 60, 17d / 6, -1d / 12, 0, 0, 0 },
                    { 2383d / 4100, 0, 0, -341d / 164, 4496d / 1025, -301d / 82, 2133d / 4100, 45d / 82, 45d / 164, 18d / 41, 0, 0 },
                    { 3d / 205, 0, 0, 0, 0, -6d / 41, -3d / 205, -3d / 41, 3d / 41, 6d / 41, 0, 0 },
                    { -1777d / 4100, 0, 0, -341d / 164, 4496d / 1025, -289d / 82, 2193d / 4100, 51d / 82, 33d / 164, 12d / 41, 0, 1 }, },
                new double[2, 13] {
                    { 41d / 840, 0, 0, 0, 0, 34d / 105, 9d / 35, 9d / 35, 9d / 280, 9d / 280,  41d / 840, 0, 0 },
                    { 0, 0, 0, 0, 0, 34d / 105, 9d / 35, 9d / 35, 9d / 280, 9d / 280, 0, 41d / 840, 41d / 840 }},
                new double[13] { 0, 2d / 27, 1d / 9, 1d / 6, 5d / 12, 1d / 2, 5d / 6, 1d / 6, 2d / 3, 1d / 3, 1, 0, 1 },
                new double[13] { -41d / 840, 0, 0, 0, 0, 0, 0, 0, 0, 0, -41d / 840, 41d / 840, 41d / 840 });
        else if (odeMethod == ODEMethod.RungeKutta_Fehlberg_56)
            Butcher = (new double[7, 7] {
                    { 1d / 6, 0, 0, 0, 0, 0, 0 },
                    { 4d / 75, 16d / 75, 0, 0, 0, 0, 0 },
                    { 5d / 6, -8d / 3, 5d / 2, 0, 0, 0, 0 },
                    { -8d / 5, 144d / 25, -4, 16d / 25, 0, 0, 0 },
                    { 361d / 320, -18d / 5, 407d / 128, -11d / 80, 55d / 128, 0, 0 },
                    { -11d / 640, 0, 11d / 256, -11d / 160, 11d / 256, 0, 0 },
                    { 93d / 640, -18d / 5, 803d / 256, -11d / 160, 99d / 256, 0, 1 } },
                new double[2, 8] {
                    { 31d / 384, 0, 1125d / 2816, 9d / 32, 125d / 768, 5d / 66, 0, 0 },
                    { 7d / 1408, 0, 1125d / 2816, 9d / 32, 125d / 768, 0, 5d / 66, 5d / 66 } },
                new double[8] { 0, 1d / 6, 4d / 15, 2d / 3, 4d / 5, 1, 0, 1 },
                new double[8] { -5d / 66, 0, 0, 0, 0, -5d / 66, 5d / 66, 5d / 66 });
        else if (odeMethod == ODEMethod.RungeKutta_Verner_56)
            Butcher = (new double[7, 7] {
                    { 1d / 18, 0, 0, 0, 0, 0, 0 },
                    { -1d / 12, 1d / 4, 0, 0, 0, 0, 0 },
                    { -2d / 81, 4d / 27, 8d / 81, 0, 0, 0, 0 },
                    { 40d / 33, -4d / 11, -56d / 11, 54d / 11, 0, 0, 0 },
                    { -369d / 73, 72d / 73, 5380d / 219, -12285d / 584, 2695d / 1752, 0, 0 },
                    { -8716d / 891, 656d / 297, 39520d / 891, -416d / 11, 52d / 27, 0, 0 },
                    { 3015d / 256, -9d / 4, -4219d / 78, 5985d / 128, -539d / 384, 0, 693d / 3328 } },
                new double[2, 8] {
                    { 3d / 80, 0, 4d / 25, 243d / 1120, 77d / 160, 73d / 700, 0, 0 },
                    { 57d / 640, 0, -16d / 65, 1377d / 2240, 121d / 320, 0, 891d / 8320, 2d / 35 } },
                new double[8] { 0, 1d / 18, 1d / 6, 2d / 9, 2d / 3, 1, 8d / 9, 1 },
                new double[8] { 33d / 640, 0, -132d / 325, 891d / 2240, -33d / 320, -73d / 700, 891d / 8320, 2d / 35 });
        else if (odeMethod == ODEMethod.RungeKutta_BogackiShampine_45)
            Butcher = (new double[7, 7] {
                    { 1d / 6, 0, 0, 0, 0, 0, 0 },
                    { 2d / 27, 4d / 27, 0, 0, 0, 0, 0 },
                    { 183d / 1372, -162d / 343, 1053d / 1372, 0, 0, 0, 0 },
                    { 68d / 297, -4d / 11, 42d / 143, 1960d / 3861, 0, 0, 0 },
                    { 597d / 22528, 81d / 352, 63099d / 585728, 58653d / 366080, 4617d / 20480, 0, 0 },
                    { 174197d / 959244, -30942d / 79937, 8152137d / 19744439, 666106d / 1039181, -29421d / 29068, 482048d / 414219, 0 },
                    { 587d / 8064, 0, 4440339d / 15491840, 24353d / 124800, 387d / 44800, 2152d / 5985, 7267d / 94080 } },
                new double[2, 8] {
                    { 587d / 8064, 0, 4440339d / 15491840, 24353d / 124800, 387d / 44800, 2152d / 5985, 7267d / 94080, 0 },
                    { 2479d / 34992, 0, 123d / 416, 612941d / 3411720, 43d / 1440, 2272d / 6561,  79937d / 1113912, 3293d / 556956 } },
                new double[8] { 0, 1d / 6, 2d / 9, 3d / 7, 2d / 3, 3d / 4, 1, 1 },
                new double[8] { -3817d / 1959552, 0, 140181d / 15491840, -4224731d / 272937600, 8557d / 403200, -57928d / 4363065, -23930231d / 4366535040, 3293d / 556956 });
        else if (odeMethod == ODEMethod.RungeKutta_DormandPrince_45)
            Butcher = (new double[6, 6] {
                    { 1d / 5, 0, 0, 0, 0, 0 },
                    { 3d / 40, 9d / 40, 0, 0, 0, 0 },
                    { 44d / 45, -56d / 15, 32d / 9, 0, 0, 0 },
                    { 19372d / 6561, -25360d / 2187, 64448d / 6561, -212d / 729, 0, 0 },
                    { 9017d / 3168, -355d / 33, 46732d / 5247, 49d / 176, -5103d / 18656, 0 },
                    { 35d / 384, 0, 500d / 1113, 125d / 192, -2187d / 6784, 11d / 84 } },
                new double[2, 7] { 
                    { 35d / 384, 0, 500d / 1113, 125d / 192, -2187d / 6784, 11d / 84, 0 },
                    { 5179d / 57600, 0, 7571d / 16695, 393d / 640, -92097d / 339200, 187d / 2100, 1d / 40 } },
                new double[7] { 0, 1d / 5, 3d / 10, 4d / 5, 8d / 9, 1, 1 },
                new double[7] { -71d / 57600, 0, 71d / 16695, -71d / 1920, 17253d / 339200, -22d / 525, 1d / 40 });
        else if (odeMethod == ODEMethod.RungeKutta_DormandPrince_45_1)
            Butcher = (new double[6, 6] {
                    { 2d / 9, 0, 0, 0, 0, 0 },
                    { 1d / 12, 1d / 4, 0, 0, 0, 0 },
                    { 55d / 324, -25d / 105, 50d / 81, 0, 0, 0 },
                    { 83d / 330, -13d / 22, 61d / 66, 9d / 110, 0, 0 },
                    { -19d / 28, 9d / 4, 1d / 7, -27d / 7, 22d / 7, 0 },
                    { 19d / 200, 0, 3d / 5, -243d / 400, 33d / 40, 7d / 80 } },
                new double[2, 7] {
                    { 19d / 200, 0, 3d / 5, -243d / 400, 33d / 40, 7d / 80, 0 },
                    { 431d / 5000, 0, 333d / 500, -7857d / 10000, 957d / 1000, 193d / 2000, -1d / 50 } },
                new double[7] { 0, 1d / 5, 3d / 10, 4d / 5, 8d / 9, 1, 1 },
                new double[7] { -11d / 1250, 0, 33d / 500, -891d / 5000, 33d / 250, 9d / 1000, -1d / 50 });
        else if (odeMethod == ODEMethod.RungeKutta_Fehlberg_54)
            Butcher = (new double[5, 5] {
                    { 1d / 4, 0, 0, 0, 0 },
                    { 3d / 32, 9d / 32, 0, 0, 0 },
                    { 1932d / 2197, -7200d / 2197, 7296d / 2197, 0, 0 },
                    { 439d / 216, -8, 3680d / 513, -845d / 4104, 0 },
                    { -8d / 27, 2, -3544d / 2565, 1859d / 4104, -11d / 40 } },
                new double[2, 6] { 
                    { 16d / 135, 0, 6656d / 12825, 28561d / 56430, -9d / 50, 2d / 55 },
                    { 25d / 216, 0, 1408d / 2565, 2197d / 4104, -1d / 5, 0 } },
                new double[6] { 0, 1d / 4, 3d / 8, 12d / 13, 1, 1d / 2 },
                new double[6] { 1d / 360, 0, -128d / 4275, -2197d / 75240, 1d / 50, 2d / 55 });
        else if (odeMethod == ODEMethod.RungeKutta_England_54)
            Butcher = (new double[5, 5] {
                    { 1d / 2, 0, 0, 0,0 },
                    { 1d / 4, 1d / 4, 0, 0,0 },
                    { 0, -1, 2, 0,0 },
                    { 7d / 27, 10d / 27, 0, 1d / 27, 0 },
                    { 28d / 625, -1d / 5, 546d / 625, 54d / 625, -378d / 625 } },
                new double[2, 6] { 
                    { 1d / 24, 0, 0, 5d / 48, 27d / 56, 125d / 336 },
                    { 1d / 6, 0, 2d / 3, 1d / 6, 0, 0 } },
                new double[6] { 0, 1d / 2, 1d / 2, 1, 2d / 3, 1d / 5 },
                new double[6] { -1d / 8, 0, -2d / 3, -1d / 16, 27d / 56, 125d / 336 });
        else if (odeMethod == ODEMethod.RungeKutta_CashKarp_54)
            Butcher = (new double[5, 5] {
                    { 1d / 5, 0, 0, 0,0 },
                    { 3d / 40, 9d / 40, 0, 0,0 },
                    { 3d / 10, -9d / 10, 6d / 5, 0,0 },
                    { -11d / 54, 5d / 2, -70d / 27, 35d / 27,0 },
                    { 1631d / 55296, 175d / 512, 575d / 13824, 44275d / 110592, 253d / 4096} },
                new double[2, 6] { 
                    { 37d / 378, 0, 250d / 621, 125d / 594, 0, 512d / 1771},
                    { 2825d / 27648, 0, 18575d / 48384, 13525d / 55296, 277d / 14336, 1d / 4 } },
                new double[6] { 0, 1d / 5, 3d / 10, 3d / 5, 1, 7d / 8 },
                new double[6] { -277d / 64512, 0, 6925d / 370944, -6925d / 202752, -277d / 14336, 277d / 7084 });
        else // (odeMethod == ODEMethod.RungeKutta_Merson_45)
            Butcher = (new double[4, 4] {
                    { 1d / 3, 0, 0, 0 },
                    { 1d / 6, 1d / 6, 0, 0 },
                    { 1d / 8, 0, 3d / 8, 0 },
                    { 1d / 2, 0, -3d / 2, 2 } },
                new double[2, 5] { 
                    { 1d / 2, 0, -3d / 2, 2, 0 },
                    { 1d / 6, 0, 0, 2d / 3, 1d / 6 } },
                new double[5] { 0, 1d / 3, 1d / 3, 1, 1 },
                new double[5] { -1d / 15, 0, 3d / 10, -4d / 15, 1d / 30 });
        (EulerAngles, DimensionlessPulses)[] result = new (EulerAngles, DimensionlessPulses)[nu.Length];
        result[0] = initialValues;
        double nu_now = 0;
        //double delta_nu_temp = delta_nu;
        //double delta_nu_temp1;
        //double nu_now_temp;
        (EulerAngles, DimensionlessPulses) temp;
        for (int i = 1; nu_now < nu[^1];)
        {
            //if (delta_nu > nu[i] - nu_now)
            //{
            //    delta_nu = nu[i] - nu_now;
            //    temp = rk_adapt(result[i - 1], ODEMotions, odeMethod, ref nu_now, ref delta_nu, Butcher, epsilon);
            //    //отимизация
            //    //nu_now_temp = nu_now; // запоминаем время для сравнения
            //    //delta_nu_temp = nu[i] - nu_now;//шаг, с которым работаем
            //    //delta_nu_temp1 = delta_nu_temp;// для сравнения изменения шага
            //    //temp = rk_adapt(result[i - 1], ODEMotions, odeMethod, ref nu_now, ref delta_nu_temp, Butcher, epsilon);
            //    //if (nu_now - nu_now_temp < delta_nu_temp1)//
            //    //    delta_nu = delta_nu_temp;//понадобился шаг меньше
            //}
            //else
            //    temp = rk_adapt(result[i - 1], ODEMotions, odeMethod, ref nu_now, ref delta_nu, Butcher, epsilon);
            if (delta_nu > nu[i] - nu_now)
                delta_nu = nu[i] - nu_now;
            temp = rk_adapt(result[i - 1], ODEMotions, odeMethod, ref nu_now, ref delta_nu, Butcher, epsilon);
            if (nu_now >= nu[i])
            {
                result[i] = temp;
                i++;
            }
            //else if (nu_now > nu[i])
            //{
            //    Debug.LogError($"ошибка в адаптивном методе {nu_now - nu[i]} при шаге {delta_nu}");
            //    EditorApplication.isPaused = true;
            //}
        }
        return result;
    }
    internal static (EulerAngles, DimensionlessPulses) RK4(in (EulerAngles angle, DimensionlessPulses impuls) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, in double t, in double delta_t, in (double[,] A, double[,] B, double[] C) Butcher)
    {
        var k = new double[Butcher.C.Length, ODEMotions.Count];
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
    internal static (EulerAngles, DimensionlessPulses) RK_adapt_step(in (EulerAngles angle, DimensionlessPulses impuls) y, List<Func<double, (EulerAngles, DimensionlessPulses), double>> ODEMotions, in ODEMethod odeMethod, ref double t, ref double delta_t, in (double[,] A, double[,] B, double[] C, double[] D) Butcher, in double epsilon)
    {
        var k = new double[Butcher.C.Length, ODEMotions.Count];
        double[] TE = new double[ODEMotions.Count];
        (EulerAngles angle, DimensionlessPulses impuls) next;
        //(EulerAngles angle, DimensionlessPulses impuls) next_lower;
        (EulerAngles angle, DimensionlessPulses impuls) control;
        int p = 5;
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
                    (new EulerAngles(y.angle.phi + Summ(Butcher.A, k, 3, 4, 0) * delta_t,
                    y.angle.psi + Summ(Butcher.A, k, 3, 4, 1) * delta_t,
                    y.angle.theta + Summ(Butcher.A, k, 3, 4, 2) * delta_t),
                    new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.A, k, 3, 4, 3) * delta_t,
                    y.impuls.ppsi + Summ(Butcher.A, k, 3, 4, 4) * delta_t,
                    y.impuls.ptheta + Summ(Butcher.A, k, 3, 4, 5) * delta_t)));
            if (odeMethod == ODEMethod.RungeKutta_Merson_45)
            {
                next = (angle: new EulerAngles(y.angle.phi + Summ(Butcher.B, k, 0, 5, 0) * delta_t,
                    y.angle.psi + Summ(Butcher.B, k, 0, 5, 1) * delta_t,
                    y.angle.theta + Summ(Butcher.B, k, 0, 5, 2) * delta_t),
                    impuls: new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.B, k, 0, 5, 3) * delta_t,
                    y.impuls.ppsi + Summ(Butcher.B, k, 0, 5, 4) * delta_t,
                    y.impuls.ptheta + Summ(Butcher.B, k, 0, 5, 5) * delta_t));
                control = (angle: new EulerAngles(Summ(Butcher.D, k, 5, 0) * delta_t,
                    Summ(Butcher.D, k, 5, 1) * delta_t,
                    Summ(Butcher.D, k, 5, 2) * delta_t),
                    impuls: new DimensionlessPulses(Summ(Butcher.D, k, 5, 3) * delta_t,
                    Summ(Butcher.D, k, 5, 4) * delta_t,
                    Summ(Butcher.D, k, 5, 5) * delta_t));
            }
            else
            {
                for (int i = 0; i < ODEMotions.Count; i++)
                    k[5, i] = ODEMotions[i](t + Butcher.C[5] * delta_t,
                        (new EulerAngles(y.angle.phi + Summ(Butcher.A, k, 4, 5, 0) * delta_t,
                        y.angle.psi + Summ(Butcher.A, k, 4, 5, 1) * delta_t,
                        y.angle.theta + Summ(Butcher.A, k, 4, 5, 2) * delta_t),
                        new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.A, k, 4, 5, 3) * delta_t,
                        y.impuls.ppsi + Summ(Butcher.A, k, 4, 5, 4) * delta_t,
                        y.impuls.ptheta + Summ(Butcher.A, k, 4, 5, 5) * delta_t)));
                if (odeMethod == ODEMethod.RungeKutta_Fehlberg_54 || odeMethod == ODEMethod.RungeKutta_England_54 || odeMethod == ODEMethod.RungeKutta_CashKarp_54)
                {
                    next = (angle: new EulerAngles(y.angle.phi + Summ(Butcher.B, k, 0, 6, 0) * delta_t,
                        y.angle.psi + Summ(Butcher.B, k, 0, 6, 1) * delta_t,
                        y.angle.theta + Summ(Butcher.B, k, 0, 6, 2) * delta_t),
                        impuls: new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.B, k, 0, 6, 3) * delta_t,
                        y.impuls.ppsi + Summ(Butcher.B, k, 0, 6, 4) * delta_t,
                        y.impuls.ptheta + Summ(Butcher.B, k, 0, 6, 5) * delta_t));
                    control = (angle: new EulerAngles(Summ(Butcher.D, k, 6, 0) * delta_t,
                        Summ(Butcher.D, k, 6, 1) * delta_t,
                        Summ(Butcher.D, k, 6, 2) * delta_t),
                        impuls: new DimensionlessPulses(Summ(Butcher.D, k, 6, 3) * delta_t,
                        Summ(Butcher.D, k, 6, 4) * delta_t,
                        Summ(Butcher.D, k, 6, 5) * delta_t));
                }
                else
                {
                    for (int i = 0; i < ODEMotions.Count; i++)
                        k[6, i] = ODEMotions[i](t + Butcher.C[6] * delta_t,
                            (new EulerAngles(y.angle.phi + Summ(Butcher.A, k, 5, 6, 0) * delta_t,
                            y.angle.psi + Summ(Butcher.A, k, 5, 6, 1) * delta_t,
                            y.angle.theta + Summ(Butcher.A, k, 5, 6, 2) * delta_t),
                            new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.A, k, 5, 6, 3) * delta_t,
                            y.impuls.ppsi + Summ(Butcher.A, k, 5, 6, 4) * delta_t,
                            y.impuls.ptheta + Summ(Butcher.A, k, 5, 6, 5) * delta_t)));
                    if (odeMethod == ODEMethod.RungeKutta_DormandPrince_45 || odeMethod == ODEMethod.RungeKutta_DormandPrince_45_1)
                    {
                        next = (angle: new EulerAngles(y.angle.phi + Summ(Butcher.B, k, 0, 7, 0) * delta_t,
                            y.angle.psi + Summ(Butcher.B, k, 0, 7, 1) * delta_t,
                            y.angle.theta + Summ(Butcher.B, k, 0, 7, 2) * delta_t),
                            impuls: new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.B, k, 0, 7, 3) * delta_t,
                            y.impuls.ppsi + Summ(Butcher.B, k, 0, 7, 4) * delta_t,
                            y.impuls.ptheta + Summ(Butcher.B, k, 0, 7, 5) * delta_t));
                        control = (angle: new EulerAngles(Summ(Butcher.D, k, 7, 0) * delta_t,
                            Summ(Butcher.D, k, 7, 1) * delta_t,
                            Summ(Butcher.D, k, 7, 2) * delta_t),
                            impuls: new DimensionlessPulses(Summ(Butcher.D, k, 7, 3) * delta_t,
                            Summ(Butcher.D, k, 7, 4) * delta_t,
                            Summ(Butcher.D, k, 7, 5) * delta_t));
                    }
                    else
                    {
                        p = 6;
                        for (int i = 0; i < ODEMotions.Count; i++)
                            k[7, i] = ODEMotions[i](t + Butcher.C[7] * delta_t,
                            (new EulerAngles(y.angle.phi + Summ(Butcher.A, k, 6, 7, 0) * delta_t,
                            y.angle.psi + Summ(Butcher.A, k, 6, 7, 1) * delta_t,
                            y.angle.theta + Summ(Butcher.A, k, 6, 7, 2) * delta_t),
                            new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.A, k, 6, 7, 3) * delta_t,
                            y.impuls.ppsi + Summ(Butcher.A, k, 6, 7, 4) * delta_t,
                            y.impuls.ptheta + Summ(Butcher.A, k, 6, 7, 5) * delta_t)));
                        if (odeMethod == ODEMethod.RungeKutta_BogackiShampine_45 || odeMethod == ODEMethod.RungeKutta_Verner_56 || odeMethod == ODEMethod.RungeKutta_Fehlberg_56)
                        {
                            next = (angle: new EulerAngles(y.angle.phi + Summ(Butcher.B, k, 0, 8, 0) * delta_t,
                                y.angle.psi + Summ(Butcher.B, k, 0, 8, 1) * delta_t,
                                y.angle.theta + Summ(Butcher.B, k, 0, 8, 2) * delta_t),
                                impuls: new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.B, k, 0, 8, 3) * delta_t,
                                y.impuls.ppsi + Summ(Butcher.B, k, 0, 8, 4) * delta_t,
                                y.impuls.ptheta + Summ(Butcher.B, k, 0, 8, 5) * delta_t));
                            control = (angle: new EulerAngles( Summ(Butcher.D, k, 8, 0) * delta_t,
                                Summ(Butcher.D, k, 8, 1) * delta_t,
                                Summ(Butcher.D, k, 8, 2) * delta_t),
                                impuls: new DimensionlessPulses(Summ(Butcher.D, k, 8, 3) * delta_t,
                                Summ(Butcher.D, k, 8, 4) * delta_t,
                                Summ(Butcher.D, k, 8, 5) * delta_t));
                        }
                        else
                        {
                            p = 8;
                            for (int i = 0; i < ODEMotions.Count; i++)
                                k[8, i] = ODEMotions[i](t + Butcher.C[8] * delta_t,
                                (new EulerAngles(y.angle.phi + Summ(Butcher.A, k, 7, 8, 0) * delta_t,
                                y.angle.psi + Summ(Butcher.A, k, 7, 8, 1) * delta_t,
                                y.angle.theta + Summ(Butcher.A, k, 7, 8, 2) * delta_t),
                                new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.A, k, 7, 8, 3) * delta_t,
                                y.impuls.ppsi + Summ(Butcher.A, k, 7, 8, 4) * delta_t,
                                y.impuls.ptheta + Summ(Butcher.A, k, 7, 8, 5) * delta_t)));
                            for (int i = 0; i < ODEMotions.Count; i++)
                                k[9, i] = ODEMotions[i](t + Butcher.C[9] * delta_t,
                                (new EulerAngles(y.angle.phi + Summ(Butcher.A, k, 8, 9, 0) * delta_t,
                                y.angle.psi + Summ(Butcher.A, k, 8, 9, 1) * delta_t,
                                y.angle.theta + Summ(Butcher.A, k, 8, 9, 2) * delta_t),
                                new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.A, k, 8, 9, 3) * delta_t,
                                y.impuls.ppsi + Summ(Butcher.A, k, 8, 9, 4) * delta_t,
                                y.impuls.ptheta + Summ(Butcher.A, k, 8, 9, 5) * delta_t)));
                            for (int i = 0; i < ODEMotions.Count; i++)
                                k[10, i] = ODEMotions[i](t + Butcher.C[10] * delta_t,
                                (new EulerAngles(y.angle.phi + Summ(Butcher.A, k, 9, 10, 0) * delta_t,
                                y.angle.psi + Summ(Butcher.A, k, 9, 10, 1) * delta_t,
                                y.angle.theta + Summ(Butcher.A, k, 9, 10, 2) * delta_t),
                                new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.A, k, 9, 10, 3) * delta_t,
                                y.impuls.ppsi + Summ(Butcher.A, k, 9, 10, 4) * delta_t,
                                y.impuls.ptheta + Summ(Butcher.A, k, 9, 10, 5) * delta_t)));
                            for (int i = 0; i < ODEMotions.Count; i++)
                                k[11, i] = ODEMotions[i](t + Butcher.C[11] * delta_t,
                                (new EulerAngles(y.angle.phi + Summ(Butcher.A, k, 10, 11, 0) * delta_t,
                                y.angle.psi + Summ(Butcher.A, k, 10, 11, 1) * delta_t,
                                y.angle.theta + Summ(Butcher.A, k, 10, 11, 2) * delta_t),
                                new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.A, k, 10, 11, 3) * delta_t,
                                y.impuls.ppsi + Summ(Butcher.A, k, 10, 11, 4) * delta_t,
                                y.impuls.ptheta + Summ(Butcher.A, k, 10, 11, 5) * delta_t)));
                            for (int i = 0; i < ODEMotions.Count; i++)
                                k[12, i] = ODEMotions[i](t + Butcher.C[12] * delta_t,
                                (new EulerAngles(y.angle.phi + Summ(Butcher.A, k, 11, 12, 0) * delta_t,
                                y.angle.psi + Summ(Butcher.A, k, 11, 12, 1) * delta_t,
                                y.angle.theta + Summ(Butcher.A, k, 11, 12, 2) * delta_t),
                                new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.A, k, 11, 12, 3) * delta_t,
                                y.impuls.ppsi + Summ(Butcher.A, k, 11, 12, 4) * delta_t,
                                y.impuls.ptheta + Summ(Butcher.A, k, 11, 12, 5) * delta_t)));
                            next = (angle: new EulerAngles(y.angle.phi + Summ(Butcher.B, k, 0, 13, 0) * delta_t,
                                y.angle.psi + Summ(Butcher.B, k, 0, 13, 1) * delta_t,
                                y.angle.theta + Summ(Butcher.B, k, 0, 13, 2) * delta_t),
                                impuls: new DimensionlessPulses(y.impuls.pphi + Summ(Butcher.B, k, 0, 13, 3) * delta_t,
                                y.impuls.ppsi + Summ(Butcher.B, k, 0, 13, 4) * delta_t,
                                y.impuls.ptheta + Summ(Butcher.B, k, 0, 13, 5) * delta_t));
                            control = (angle: new EulerAngles(Summ(Butcher.D, k, 13, 0) * delta_t,
                                Summ(Butcher.D, k, 13, 1) * delta_t,
                                Summ(Butcher.D, k, 13, 2) * delta_t),
                                impuls: new DimensionlessPulses(Summ(Butcher.D, k, 13, 3) * delta_t,
                                Summ(Butcher.D, k, 13, 4) * delta_t,
                                Summ(Butcher.D, k, 13, 5) * delta_t));
                        }
                    }
                }
            }
            TE[0] = Math.Abs(control.angle.phi);
            TE[1] = Math.Abs(control.angle.psi);
            TE[2] = Math.Abs(control.angle.theta);
            TE[3] = Math.Abs(control.impuls.pphi);
            TE[4] = Math.Abs(control.impuls.ppsi);
            TE[5] = Math.Abs(control.impuls.ptheta);
            if (TE.Max() <= epsilon)
            {
                t += delta_t;
                if (TE.Max() != 0)
                    delta_t *= 0.9 * Math.Pow(epsilon / TE.Max(), 1d / p);
                else
                    delta_t *= 2;
                //if (double.IsInfinity(delta_t) || delta_t == 0)
                //    Debug.LogError($"шаг стал бесконечным: {double.IsInfinity(delta_t)}");
                return next;
            }
            else
                delta_t *= 0.9 * Math.Pow(epsilon / TE.Max(), 1d / p);
        }
    }
    private static double Summ(in double[,] Butcher, in double[,] k, int line, int column, int lineEquations)
    {
        double sum = 0;
        for (int i = 0; i < column; i++)
            sum += Butcher[line, i] * k[i, lineEquations];
        return sum;
    }
    private static double Summ(in double[] Butcher, in double[,] k, int column, int lineEquations)
    {
        double sum = 0;
        for (int i = 0; i < column; i++)
            sum += Butcher[i] * k[i, lineEquations];
        return sum;
    }
}