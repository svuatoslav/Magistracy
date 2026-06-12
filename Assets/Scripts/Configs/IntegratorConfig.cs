using System;

[Serializable]
public sealed class IntegratorConfig
{
    public int ApproximationKeplerEquation { get; }
    public int KeplerSolver { get; }

    public double RKIntegrationStep { get; }
    public double RKEpsilon { get; }
    public int RKSolver { get; }
    public int RKError { get; }

    public IntegratorConfig(
        int approximationKeplerEquation, int keplerSolver,
        double rkIntegrationStep, double rkEpsilon, int rkSolver, int rKError)
    {
        ApproximationKeplerEquation = approximationKeplerEquation;
        KeplerSolver = keplerSolver;
        RKIntegrationStep = rkIntegrationStep;
        RKEpsilon = rkEpsilon;
        RKSolver = rkSolver;
        RKError = rKError;
    }
}
