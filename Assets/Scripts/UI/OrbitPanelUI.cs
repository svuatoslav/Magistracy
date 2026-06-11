using System.Globalization;
using TMPro;
using UnityEngine;

public sealed class OrbitPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField eccentricityInput;

    public bool TryGetData(out OrbitConfig data, out string error)
    {
        data = null;
        error = string.Empty;

        if (!TryReadDouble(eccentricityInput, out double e) || e < 0 || e >= 1)
            return Fail("Эксцентриситет должен быть в диапазоне [0; 1).", out error);

        data = new OrbitConfig(0, e);
        return true;
    }

    private bool TryReadDouble(TMP_InputField input, out double value)
    {
        value = 0;
        if (input == null || string.IsNullOrWhiteSpace(input.text)) return false;

        string normalized = input.text.Trim().Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private bool Fail(string message, out string error)
    {
        error = message;
        return false;
    }
}