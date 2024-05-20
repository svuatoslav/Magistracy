using DATA;
using System.Collections;
using System.Collections.Generic;
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
        //LocalAxes.transform.rotation = Quaternion.Euler(new Vector3((float)SatelliteScript.Angle[0].phi, (float)SatelliteScript.Angle[0].psi, (float)SatelliteScript.Angle[0].theta));
    }
    private void FixedUpdate()
    {
        OrbitalAxes.transform.position = Satellite.transform.position;
        OrbitalAxes.transform.rotation = Quaternion.Euler(new Vector3(0, -(float)Run.Instance.data.Nu[SatelliteScript.TimeIndex] * Mathf.Rad2Deg, 0));
        LocalAxes.transform.rotation = Quaternion.Euler(new Vector3((float)SatelliteScript.Angle[SatelliteScript.TimeIndex].phi, (float)SatelliteScript.Angle[SatelliteScript.TimeIndex].psi - (float)Run.Instance.data.Nu[SatelliteScript.TimeIndex] * Mathf.Rad2Deg, (float)SatelliteScript.Angle[SatelliteScript.TimeIndex].theta));
        // цилиндр
        //LocalAxes.transform.rotation = Quaternion.Euler(new Vector3((float)SatelliteScript.Angle[SatelliteScript.TimeIndex].psi, (float)SatelliteScript.Angle[SatelliteScript.TimeIndex].phi - (float)Run.Instance.data.Nu[SatelliteScript.TimeIndex] * Mathf.Rad2Deg, (float)SatelliteScript.Angle[SatelliteScript.TimeIndex].theta));
        //LocalAxes.transform.rotation = Quaternion.Euler(new Vector3(30, 0, 0)) * Quaternion.Euler(new Vector3(0, -(float)Run.Instance.data.Nu[SatelliteScript.TimeIndex] * Mathf.Rad2Deg, 0));
        //LocalAxes.transform.rotation = OrbitalAxes.transform.rotation * Quaternion.Euler(new Vector3((float)LocalAngle.phi, (float)LocalAngle.psi, (float)LocalAngle.theta));
    }
}
