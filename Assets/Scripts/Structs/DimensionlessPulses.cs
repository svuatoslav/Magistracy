using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.Structs
{
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other) => other is not EulerAngles ? false : Equals((EulerAngles)other);

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
        }
    }
}
