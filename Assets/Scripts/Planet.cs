using Assets.Scripts.DataSerialiizers;
using MathNet.Numerics.Distributions;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] private Satellite _satellite = null;
    [SerializeField] private double Mu = 398600.4418e9; // км³/с²
    [SerializeField] private double R = 6378136.6;  // м
    [SerializeField] private double G = 6.67428e-11; //  Н · м²/кг² или  м³⋅с−²⋅кг−1
    //internal double n => Math.Sqrt(mu * Math.Pow((1 - Eccentricity) / _satellite.r_min, 3));//Math.Sqrt(g*Math.Pow(R,2)/Math.Pow(_satellite.p / (1 - Math.Pow(Eccentricity, 2)), 3));

    public void ApplyConfig(PlanetConfig planetConfig)
    {
        R = planetConfig.Radius;
        Mu = planetConfig.Mu;
    }

    private void Awake()
    {
        var jSONManager = new JSONManager();
        ApplyConfig(jSONManager.Read<PlanetConfig>("")); // пока так
    }
}