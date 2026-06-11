using UnityEngine;
using static System.Math;

namespace Assets.Scripts
{

    public class Circular : Orbit
    {
        [SerializeField] private double _r = 0;

        public double R { get => _r; }
        public void ApplyConfig(OrbitConfig orbitConfig)
        {
            _r = orbitConfig.r_pericenter;
        }

        public override double H(Satellite satellite, double nu) => (Pow(satellite.Ppsi / Sin(satellite.Theta), 2)
            + Pow(satellite.Ptheta, 2)) / 2 - (satellite.Alpha * satellite.Beta / Sin(satellite.Theta) + Cos(satellite.Psi)) * satellite.Ppsi / Tan(satellite.Theta)
            - Sin(satellite.Psi) * satellite.Ptheta + Pow(satellite.Alpha * satellite.Beta / Tan(satellite.Theta), 2) / 2 + satellite.Alpha * satellite.Beta
            * Cos(satellite.Psi) / Sin(satellite.Theta) + (3 * (satellite.Alpha - 1)) * Pow(Cos(satellite.Theta), 2) / 2;

        public override double Diff_phi(Satellite satellite, double nu) => Omega0 * (satellite.Beta + Cos(satellite.Psi0) * Sin(satellite.Theta0));

        public override double Diff_psi(Satellite satellite, double nu) => (satellite.Ppsi - satellite.Alpha * satellite.Beta * Cos(satellite.Theta)) / Pow(Sin(satellite.Theta), 2)
            - Cos(satellite.Psi) / Tan(satellite.Theta);

        public override double Diff_theta(Satellite satellite, double nu) => satellite.Ptheta - Sin(satellite.Psi);

        public override double Diff_pphi(Satellite satellite, double nu) => 0;

        public override double Diff_ppsi(Satellite satellite, double nu) => Sin(satellite.Psi) * (satellite.Alpha * satellite.Beta - Cos(satellite.Theta) * satellite.Ppsi)
            / Sin(satellite.Theta) + Cos(satellite.Psi) * satellite.Ptheta;

        public override double Diff_ptheta(Satellite satellite, double nu) => ((Pow(satellite.Alpha * satellite.Beta * Cos(satellite.Theta) - satellite.Ppsi, 2) / Sin(satellite.Theta)
            + Cos(satellite.Psi) * (satellite.Alpha * satellite.Beta - satellite.Ppsi * Cos(satellite.Theta))) / Tan(satellite.Theta) + satellite.Alpha * satellite.Beta * (satellite.Alpha * satellite.Beta * Cos(satellite.Theta)
            - satellite.Ppsi)) / Sin(satellite.Theta) + ((3 * (satellite.Alpha - 1)) * Sin(2 * satellite.Theta)) / 2 - satellite.Ppsi * Cos(satellite.Psi);

        private protected override Vector3[] DrawOrbit(double step, int scale)//Scale
        {
            Vector3[] positions = new Vector3[(int)(360 / step) + 1];
            for (int i = 0; i < positions.Length; i++)
                positions[i] = new Vector3(-(float)((R / scale) * Sin((i * step) * Mathf.Deg2Rad)), 0, (float)((R / scale) * Cos((i * step) * Mathf.Deg2Rad)));
            return positions;
        }
    }
}
