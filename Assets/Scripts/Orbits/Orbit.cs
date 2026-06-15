using Assets;
using Assets.Scripts.DataSerialiizers;
using UnityEngine;

public abstract class Orbit : MonoBehaviour, IODEMotions, IApplyConfig<OrbitConfig>
{
    [SerializeField] protected Planet _planet = null;
    [SerializeField] protected Satellite _Satellite = null;
    [SerializeField] protected double _omega0 = 0;
    [SerializeField] protected double _beta = 0;
    public double Omega0 { get => _omega0; protected set => _omega0 = value; }

    protected LineRenderer lineRenderer;

    protected void Awake() // проверить
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        lineRenderer.positionCount = (int)(360d / 1) + 1;
        lineRenderer.SetPositions(DrawOrbit(1, Run.Instance.Scale));

        var jSONManager = new JSONManager();
        ApplyConfig(jSONManager.Read<OrbitConfig>("")); // дополнить путь
    }

    public abstract void ApplyConfig(OrbitConfig orbitConfig);

    public abstract double H(Satellite satellite, double nu);

    public abstract double Diff_phi(Satellite satellite, double nu);

    public abstract double Diff_psi(Satellite satellite, double nu);

    public abstract double Diff_theta(Satellite satellite, double nu);

    public abstract double Diff_pphi(Satellite satellite, double nu);

    public abstract double Diff_ppsi(Satellite satellite, double nu);

    public abstract double Diff_ptheta(Satellite satellite, double nu);

    private protected abstract Vector3[] DrawOrbit(double deltaTime, int scale);
}
