using System;

[Serializable]
public sealed class IntegratorConfig
{
    public double KeplerParam1 { get; }
    public double KeplerParam2 { get; }
    public double KeplerParam3 { get; }

    public double RKParam1 { get; }
    public double RKParam2 { get; }
    public double RKParam3 { get; }

    public IntegratorConfig(
        double keplerParam1, double keplerParam2, double keplerParam3,
        double rkParam1, double rkParam2, double rkParam3)
    {
        KeplerParam1 = keplerParam1;
        KeplerParam2 = keplerParam2;
        KeplerParam3 = keplerParam3;
        RKParam1 = rkParam1;
        RKParam2 = rkParam2;
        RKParam3 = rkParam3;
    }
}
