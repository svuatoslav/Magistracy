using UnityEngine;
using static System.Math;

namespace Assets.Scripts.Orbits
{
    public class Elliptic : Orbit
    {
        [SerializeField] private double _a = 0;
        [SerializeField] private double _b = 0;
        [SerializeField] private double _eccentricity = 0;
        [SerializeField] private double _r_pericenter = 0;
        [SerializeField] private double _r_apocenter = 0;
        [SerializeField] private double _focalParameter = 0;
        [SerializeField] private double _focalLength = 0;

        public double A => _a;
        public double B => _b;
        public double E => _eccentricity;
        public double R_p => _r_pericenter;
        public double R_a => _r_apocenter;
        public double P => _focalParameter;
        public double C => _focalLength;

        public void ApplyConfig(OrbitConfig orbitConfig)
        {
            _eccentricity = orbitConfig.Eccentricity;
            _r_pericenter = orbitConfig.r_pericenter;

            _a = orbitConfig.r_pericenter / (1 - E);
            _b = orbitConfig.r_pericenter * Sqrt((1 + orbitConfig.Eccentricity) / (1 - orbitConfig.Eccentricity));
            _focalLength = orbitConfig.r_pericenter * orbitConfig.Eccentricity / (1 - orbitConfig.Eccentricity);
            _focalParameter = orbitConfig.r_pericenter * (1 + orbitConfig.Eccentricity);
            _r_apocenter = orbitConfig.r_pericenter * (1 + orbitConfig.Eccentricity) / (1 - orbitConfig.Eccentricity);
        }

        public static double EtoNu(double E, double e) => Atan2(Pow(Sqrt(1 - e), 2) * Sin(E) / (1 - e * Cos(E)), (Cos(E) - e) / (1 - e * Cos(E))); //2 * Atan(Sqrt((1 + e) / (1 - e)) * Tan(E / 2));

        public override double H(Satellite satellite, double nu) => Pow(satellite.Ppsi, 2) / (2 * Pow(1 + E * Cos(nu), 2)
        * Pow(Sin(satellite.Theta), 2)) + Pow(satellite.Ptheta, 2) / (2 * Pow(1 + E * Cos(nu), 2)) - (satellite.Alpha * satellite.Beta * Pow(1 - Pow(E, 2), 3d / 2)
        * Cos(satellite.Theta) / (Pow(1 + E * Cos(nu), 2) * Pow(Sin(satellite.Theta), 2)) + Cos(satellite.Psi) / Tan(satellite.Theta))
        * satellite.Ppsi - Sin(satellite.Psi) * satellite.Ptheta + Pow(satellite.Alpha, 2) * Pow(satellite.Beta, 2) * Pow(1 - Pow(E, 2), 3)
        / (Pow(Tan(satellite.Theta), 2) * 2 * Pow(1 + E * Cos(nu), 2)) + satellite.Alpha * satellite.Beta * Pow(1 - Pow(E, 2), 3d / 2) * Cos(satellite.Psi)
        / Sin(satellite.Theta) + 3 * (satellite.Alpha - 1) * (1 + E * Cos(nu)) * Pow(Cos(satellite.Theta), 2) / 2;

        public override double Diff_phi(Satellite satellite, double nu) => R_p + Omega(nu) * Cos(satellite.Psi) * Sin(satellite.Theta) 
            - Diff_psi(satellite, nu) * Cos(satellite.Theta);

        public double Diff_phiCylPrec(Satellite satellite, double nu) => Omega0 * (satellite.Beta - Pow(1 + E * Cos(nu), 2) / Pow(1 - Pow(E, 2), 3d / 2));

        public override double Diff_psi(Satellite satellite, double nu) => ((satellite.Ppsi - satellite.Alpha * satellite.Beta * Pow(1 - Pow(E, 2), 3d / 2)
            * Cos(satellite.Theta)) / (Pow(1 + E * Cos(nu), 2) * Sin(satellite.Theta)) - Cos(satellite.Psi) * Cos(satellite.Theta)) / Sin(satellite.Theta);

        public override double Diff_theta(Satellite satellite, double nu) => satellite.Ptheta / Pow(1 + E * Cos(nu), 2) - Sin(satellite.Psi);

        public override double Diff_pphi(Satellite satellite, double nu) => 0;

        public override double Diff_ppsi(Satellite satellite, double nu) => Sin(satellite.Psi) * (satellite.Alpha * satellite.Beta * Pow(1 - Pow(E, 2), 3d / 2)
            - Cos(satellite.Theta) * satellite.Ppsi) / Sin(satellite.Theta) + Cos(satellite.Psi) * satellite.Ptheta; 

        public override double Diff_ptheta(Satellite satellite, double nu) => Cos(satellite.Theta) 
            * (((satellite.Ppsi - satellite.Alpha * satellite.Beta * Pow(1 - Pow(E, 2), 3d / 2) * Cos(satellite.Theta))
            * ((satellite.Ppsi - satellite.Alpha * satellite.Beta * Pow(1 - Pow(E, 2), 3d / 2) * Cos(satellite.Theta)) 
            / Pow(Sin(satellite.Theta), 2) - satellite.Alpha * satellite.Beta * Pow(1 - Pow(E, 2), 3d / 2) / Cos(satellite.Theta))
            / Pow(1 + E * Cos(nu), 2) + Cos(satellite.Psi) 
            * (satellite.Alpha * satellite.Beta * Pow(1 - Pow(E, 2), 3d / 2) - Cos(satellite.Theta) * satellite.Ppsi) 
            / Sin(satellite.Theta)) / Sin(satellite.Theta) + 3 * (satellite.Alpha - 1) * (1 + E * Cos(nu)) * Sin(satellite.Theta)) 
            - Cos(satellite.Psi) * satellite.Ppsi;

        private double Omega(double nu) => Omega0 * Pow(1 + E * Cos(nu), 2) / Pow(1 - Pow(E, 2), 3d / 2);

        private protected override Vector3[] DrawOrbit(double step, int scale)
        {
            Vector3[] positions = new Vector3[(int)(360 / step) + 1];
            for (int i = 0; i < positions.Length; i++)
                positions[i] = new Vector3(-(float)((B / scale) * Sin((i * step) * Mathf.Deg2Rad)), 0, (float)((A / scale) * Cos((i * step) * Mathf.Deg2Rad)));
            return positions;
        }
    }
}
