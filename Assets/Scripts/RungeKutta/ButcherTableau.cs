namespace Assets.Scripts.RungeKutta
{
    public class ButcherTableau
    {
        public double[,] A { get; }

        public double[] B { get; }

        public double[] C { get; }

        public int Order { get; }

        public ButcherTableau(double[,] a, double[] b, double[] c, int order)
        {
            A = (double[,])a.Clone();
            B = (double[])b.Clone();
            C = (double[])c.Clone();
            Order = order;
        }
    }
}
