using System;

[Serializable]
public sealed class SimulationConfig
{
    public PlanetConfig Planet { get; }
    public SatelliteConfig Satellite { get; }
    public OrbitConfig Orbit { get; }
    public IntegratorConfig Integrator { get; }

    public int Time {  get; }
    public int Scale { get; }

    public SimulationConfig(
        PlanetConfig planet,
        SatelliteConfig satellite,
        OrbitConfig orbit,
        IntegratorConfig integrator,
        int time,
        int scale)
    {
        Planet = planet;
        Satellite = satellite;
        Orbit = orbit;
        Integrator = integrator;
        Time = time;
        Scale = scale;
    }
}
