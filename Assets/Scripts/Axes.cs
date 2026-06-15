using Assets.Scripts.Structs;
using UnityEngine;

public class Axes : MonoBehaviour
{
    [SerializeField] private GameObject Satellite = null;
    [SerializeField] private GameObject OrbitalAxes = null;
    [SerializeField] private GameObject LocalAxes = null;
    private EulerAngles Angle = EulerAngles.zero;
    private EulerAngles LocalAngle = EulerAngles.zero;
    private void Awake()
    {

    }
    void Start()
    {
        OrbitalAxes.transform.position = Satellite.transform.position;
    }
    private void FixedUpdate()
    {
        OrbitalAxes.transform.position = Satellite.transform.position;
        //OrbitalAxes.transform.rotation = Quaternion.Euler(new Vector3(0, -(float)Run.Instance.data.Nu[SatelliteScript._timeIndex] * Mathf.Rad2Deg, 0));
        //LocalAxes.transform.rotation = Quaternion.Euler(new Vector3((float)SatelliteScript.Angle[SatelliteScript._timeIndex].phi, (float)SatelliteScript.Angle[SatelliteScript._timeIndex].psi - (float)Run.Instance.data.Nu[SatelliteScript._timeIndex] * Mathf.Rad2Deg, (float)SatelliteScript.Angle[SatelliteScript._timeIndex].theta));
    }
}
