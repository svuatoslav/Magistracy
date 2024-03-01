using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using static System.Math;
namespace DATA
{
    public class Data
    {
        internal readonly Planet.Orbits orbit;
        internal readonly RegularPrecessions regularPrecession;
        internal (EulerAngles, DimensionlessPulses)[] MotionsAngle;
        internal double[] Nu;
        internal double[] H;
        public Data(RegularPrecessions regularPrecession)
        {
            this.regularPrecession = regularPrecession;
        }
    }
    public enum RegularPrecessions
    {
        Cylindrical,
        Hyperboloidal,
        Conical,
        MyParametrs
    }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles operator +(EulerAngles a, EulerAngles b) => new(a.phi + b.phi, a.psi + b.psi, a.theta + b.theta);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles operator -(EulerAngles a, EulerAngles b) => new(a.phi + b.phi, a.psi - b.psi, a.theta - b.theta);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles operator -(EulerAngles a) => new(0.0 - a.phi, 0.0 - a.psi, 0.0 - a.theta);

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
        //public override bool Equals(object other)
        //{
        //    if (other is not EulerAngles)
        //    {
        //        return false;
        //    }

        //    return Equals((EulerAngles)other);
        //}

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

            return string.Format("(phi: {0}, psi: {1}, theta: {2})", phi == 0 ? 0 : phi.ToString(format, formatProvider), psi == 0 ? 0 : psi.ToString(format, formatProvider), theta == 0 ? 0 : theta.ToString(format, formatProvider));
            //return string.Format("(phi: {0}, psi: {1}, theta: {2})", phi.ToString(format, formatProvider), psi.ToString(format, formatProvider), theta.ToString(format, formatProvider));
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static Vector3 Normalize(Vector3 value)
        //{
        //    float num = Magnitude(value);
        //    if (num > 1E-05f)
        //    {
        //        return value / num;
        //    }

        //    return zero;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void Normalize()
        //{
        //    float num = Magnitude(this);
        //    if (num > 1E-05f)
        //    {
        //        this /= num;
        //    }
        //    else
        //    {
        //        this = zero;
        //    }
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerAngles ToUnityAngle(EulerAngles value)
        {
            double alpha, beta, gamma;
            double betaSin, betaCos, gammaSin, gammaCos;
            alpha = Asin(Sin(value.theta) * Cos(value.phi));
            if (Sqrt(1 - Pow(Sin(value.theta) * Cos(value.phi), 2)) == 0)
            {
                Debug.LogError($"Не удалось перевести в Углы Эйлера используемых в Unity\nЗнаменатель Sqrt(1 - ((Sin(value.theta) * Cos(value.phi)) ^ 2) = 0 при {value}");
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
        public static EulerAngles ToDegrees(EulerAngles value) => new EulerAngles(value.phi, value.psi, value.theta) * Mathf.Rad2Deg;
    }
    [Serializable]
    public struct DimensionlessPulses : IEquatable<DimensionlessPulses>, IFormattable
    {
        public double pphi;
        public double ppsi;
        public double ptheta;
        public DimensionlessPulses(double pphi, double ppsi, double ptheta)
        {
            this.pphi = pphi;
            this.ppsi = ppsi;
            this.ptheta = ptheta;
        }
        public DimensionlessPulses(double ppsi, double ptheta)
        {
            pphi = 0;
            this.ppsi = ppsi;
            this.ptheta = ptheta;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DimensionlessPulses operator +(DimensionlessPulses a, DimensionlessPulses b) => new(a.pphi + b.pphi, a.ppsi + b.ppsi, a.ptheta + b.ptheta);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DimensionlessPulses operator -(DimensionlessPulses a, DimensionlessPulses b) => new(a.pphi - b.pphi, a.ppsi - b.ppsi, a.ptheta - b.ptheta);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DimensionlessPulses operator -(DimensionlessPulses a) => new(0.0 - a.pphi, 0.0 - a.ppsi, 0.0 - a.ptheta);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DimensionlessPulses operator *(DimensionlessPulses a, double d) => new(a.pphi * d, a.ppsi * d, a.ptheta * d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DimensionlessPulses operator *(double d, DimensionlessPulses a) => new(a.pphi * d, a.ppsi * d, a.ptheta * d);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DimensionlessPulses operator /(DimensionlessPulses a, double d) => new(a.pphi / d, a.ppsi / d, a.ptheta / d);
        public override bool Equals(object other) => other is not EulerAngles ? false : Equals((EulerAngles)other);
        //public override bool Equals(object other)
        //{
        //    if (other is not DimensionlessPulses)
        //    {
        //        return false;
        //    }

        //    return Equals((EulerAngles)other);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DimensionlessPulses other) => pphi.Equals(other.pphi) && ppsi.Equals(other.ppsi) && ptheta.Equals(other.ptheta);

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

            return string.Format("(pphi: {0}, ppsi: {1}, ptheta: {2})", pphi == 0 ? 0 : pphi.ToString(format, formatProvider), ppsi == 0 ? 0 : ppsi.ToString(format, formatProvider), ptheta == 0 ? 0 : ptheta.ToString(format, formatProvider));
            //return string.Format("(pphi: {0}, ppsi: {1}, ptheta: {2})", pphi.ToString(format, formatProvider), ppsi.ToString(format, formatProvider), ptheta.ToString(format, formatProvider));
        }
    }
}
