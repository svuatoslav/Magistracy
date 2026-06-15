using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using static System.Math;

namespace Assets.Scripts.Structs
{
    [Serializable]
    public struct EulerAngles : IEquatable<EulerAngles>, IFormattable
    {
        public double phi;
        public double psi;
        public double theta;

        public EulerAngles(double phi, double psi, double theta)
        {
            this.phi = phi;
            this.psi = psi;
            this.theta = theta;
        }

        public EulerAngles(double psi, double theta)
        {
            phi = 0;
            this.psi = psi;
            this.theta = theta;
        }

        public static EulerAngles zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return zeroEulerAngles;
            }
        }

        private static readonly EulerAngles zeroEulerAngles = new EulerAngles(0f, 0f, 0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles operator +(EulerAngles a, EulerAngles b) => new(a.phi + b.phi, a.psi + b.psi, a.theta + b.theta);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles operator -(EulerAngles a, EulerAngles b) => new(a.phi - b.phi, a.psi - b.psi, a.theta - b.theta);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles operator -(EulerAngles a) => new(-a.phi, -a.psi, -a.theta);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles operator *(EulerAngles a, double d) => new(a.phi * d, a.psi * d, a.theta * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles operator *(double d, EulerAngles a) => new(a.phi * d, a.psi * d, a.theta * d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles operator /(EulerAngles a, double d) => new(a.phi / d, a.psi / d, a.theta / d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(EulerAngles a, EulerAngles b) => a.phi == b.phi && a.psi == b.psi && a.theta == b.theta;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(EulerAngles a, EulerAngles b) => !(a == b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is not EulerAngles ? false : Equals((EulerAngles)other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(EulerAngles other) => phi.Equals(other.phi) && psi.Equals(other.psi) && theta.Equals(other.theta);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => ToString(null, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format) => ToString(format, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F10";

            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

            return string.Format("(Phi: {0}, psi: {1}, theta: {2})", phi == 0 ? 0 : phi.ToString(format, formatProvider), psi == 0 ? 0 : psi.ToString(format, formatProvider), theta == 0 ? 0 : theta.ToString(format, formatProvider));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles ToUnityAngle(EulerAngles value)
        {
            double alpha, beta, gamma;
            double betaSin, betaCos, gammaSin, gammaCos;

            alpha = Asin(Sin(value.theta) * Cos(value.phi));

            if (Sqrt(1 - Pow(Sin(value.theta) * Cos(value.phi), 2)) == 0)
            {
                //Debug.LogError($"Не удалось перевести Углы Эйлера в Unity\nЗнаменатель Sqrt(1 - ((Sin(value.theta) * Cos(value.phi)) ^ 2) = 0 \n{Sqrt(1 - Pow(Sin(value.theta) * Cos(value.phi), 2))} \nпри {value}");
                betaSin = Sin(value.phi) * Sin(value.theta) / double.Epsilon;
                betaCos = Cos(value.theta) / double.Epsilon;
                gammaSin = (Sin(value.phi) * Cos(value.psi) + Cos(value.phi) * Sin(value.psi) * Cos(value.theta)) / double.Epsilon;
                gammaCos = (Cos(value.phi) * Cos(value.psi) * Cos(value.theta) - Sin(value.phi) * Sin(value.psi)) / double.Epsilon;
            }
            else
            {
                betaSin = Sin(value.phi) * Sin(value.theta) / Sqrt(1 - Pow(Sin(value.theta) * Cos(value.phi), 2));
                betaCos = Cos(value.theta) / Sqrt(1 - Pow(Sin(value.theta) * Cos(value.phi), 2));
                gammaSin = (Sin(value.phi) * Cos(value.psi) + Cos(value.phi) * Sin(value.psi) * Cos(value.theta)) / Sqrt(1 - Pow(Sin(value.theta) * Cos(value.phi), 2));
                gammaCos = (Cos(value.phi) * Cos(value.psi) * Cos(value.theta) - Sin(value.phi) * Sin(value.psi)) / Sqrt(1 - Pow(Sin(value.theta) * Cos(value.phi), 2));
            }

            beta = Atan2(betaSin, betaCos);
            gamma = Atan2(gammaSin, gammaCos);

            return new EulerAngles(alpha, beta, gamma);
        }

        public static EulerAngles RotationAroundY(EulerAngles value, double angle)
        {
            value.phi = value.phi * Cos(angle) - value.theta * Sin(angle);
            value.theta = value.phi * Sin(angle) + value.theta * Cos(angle);
            return value;
        }

        public static EulerAngles ToDegrees(EulerAngles value) => new EulerAngles(value.phi, value.psi, value.theta) * Mathf.Rad2Deg;
    }
}
