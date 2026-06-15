using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class SimulationSetupUI : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string simulationSceneName = "SimulationScene";

    [Header("UI")]
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Panels")]
    [SerializeField] private PlanetPanelUI planetPanel;
    [SerializeField] private SatellitePanelUI satellitePanel;
    [SerializeField] private OrbitPanelUI orbitPanel;
    [SerializeField] private IntegratorPanelUI integratorPanel;

    private void Awake()
    {
        startButton.onClick.AddListener(OnStartClicked);
        SetError(string.Empty);
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(OnStartClicked);
    }

    public void StartSimulation()
    {
        throw new NotImplementedException();
    }

    private void OnStartClicked()
    {
        if (!planetPanel.TryGetData(out PlanetConfig planet, out string error)) { SetError(error); return; }
        if (!satellitePanel.TryGetData(out SatelliteConfig satellite, out error)) { SetError(error); return; }
        if (!orbitPanel.TryGetData(out OrbitConfig orbit, out error)) { SetError(error); return; }
        if (!integratorPanel.TryGetData(out IntegratorConfig integrator, out error)) { SetError(error); return; }

        //SimulationContext.Initialize(new SimulationConfig(planet, satellite, orbit, integrator));
        SceneManager.LoadScene(simulationSceneName);
    }

    private void SetError(string message)
    {
        if (errorText == null) return;
        errorText.text = message;
        errorText.gameObject.SetActive(!string.IsNullOrWhiteSpace(message));
    }
}