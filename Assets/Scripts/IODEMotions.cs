public interface IODEMotions
{
    double Diff_phi(Satellite satellite, double nu);
    double Diff_psi(Satellite satellite, double nu);
    double Diff_theta(Satellite satellite, double nu);
    double Diff_pphi(Satellite satellite, double nu);
    double Diff_ppsi(Satellite satellite, double nu);
    double Diff_ptheta(Satellite satellite, double nu);
    double H(Satellite satellite, double nu);
}