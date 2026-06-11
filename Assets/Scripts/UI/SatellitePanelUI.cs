using System.Globalization;
using TMPro;
using UnityEngine;

public sealed class SatellitePanelUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] paramsInputs = new TMP_InputField[9];

    public bool TryGetData(out SatelliteConfig data, out string error)
    {
        data = null;
        error = string.Empty;

        if (paramsInputs == null || paramsInputs.Length != 9)
            return Fail("У спутника должно быть 9 полей.", out error);

        double[] values = new double[9];

        for (int i = 0; i < 9; i++)
        {
            if (!TryReadDouble(paramsInputs[i], out values[i]))
                return Fail($"Параметр спутника {i + 1}: некорректное значение.", out error);
        }

        data = new SatelliteConfig(
            values[0], values[1], values[2],
            values[3], values[4], values[5],
            values[6], values[7], values[8]);

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