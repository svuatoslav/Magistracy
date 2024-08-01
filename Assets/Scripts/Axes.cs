using DATA;
using UnityEngine;

public class Axes : MonoBehaviour
{
    [SerializeField] private GameObject Satellite = null;
    [SerializeField] private GameObject OrbitalAxes = null;
    [SerializeField] private GameObject LocalAxes = null;
    private Satellite SatelliteScript = null;
    private EulerAngles Angle = EulerAngles.zero;
    private EulerAngles LocalAngle = EulerAngles.zero;
    private void Awake()
    {
        SatelliteScript = Satellite.GetComponent<Satellite>();
    }
    void Start()
    {
        OrbitalAxes.transform.position = SatelliteScript.StartPosition;
    }
    private void FixedUpdate()
    {
        OrbitalAxes.transform.position = Satellite.transform.position;
        OrbitalAxes.transform.rotation = Quaternion.Euler(new Vector3(0, -(float)Run.Instance.data.Nu[SatelliteScript.TimeIndex] * Mathf.Rad2Deg, 0));
        LocalAxes.transform.rotation = Quaternion.Euler(new Vector3((float)SatelliteScript.Angle[SatelliteScript.TimeIndex].phi, (float)SatelliteScript.Angle[SatelliteScript.TimeIndex].psi - (float)Run.Instance.data.Nu[SatelliteScript.TimeIndex] * Mathf.Rad2Deg, (float)SatelliteScript.Angle[SatelliteScript.TimeIndex].theta));
    }
}
