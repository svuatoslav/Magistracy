namespace Assets.Scripts.RungeKutta
{
    public class EmbeddedButcherTableau : ButcherTableau
    {
        public double[] BLower { get; }

        public int LowerOrder { get; }

        public EmbeddedButcherTableau(
            double[,] a,
            double[] b,
            double[] bLower,
            double[] c,
            int order,
            int lowerOrder)
            : base(a, b, c, order)
        {
            BLower = (double[])bLower.Clone();
            LowerOrder = lowerOrder;
        }
    }
}
