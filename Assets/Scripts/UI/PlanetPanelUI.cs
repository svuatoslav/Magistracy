using System.Globalization;
using TMPro;
using UnityEngine;

public sealed class PlanetPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField massMantissaInput;
    [SerializeField] private TMP_InputField massExponentInput;
    [SerializeField] private TMP_InputField radiusInput;

    public bool TryGetData(out PlanetConfig data, out string error)
    {
        data = null;
        error = string.Empty;

        if (!TryReadDouble(massMantissaInput, out double mantissa))
            return Fail("Масса планеты: некорректная мантисса.", out error);

        if (!TryReadInt(massExponentInput, out int exponent))
            return Fail("Масса планеты: некорректная степень.", out error);

        if (!TryReadDouble(radiusInput, out double radius) || radius <= 0)
            return Fail("Радиус планеты должен быть положительным числом.", out error);

        data = new PlanetConfig(mantissa * System.Math.Pow(10, exponent), radius);
        return true;
    }

    private bool TryReadDouble(TMP_InputField input, out double value)
    {
        value = 0;
        if (input == null || string.IsNullOrWhiteSpace(input.text)) return false;

        string normalized = input.text.Trim().Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private bool TryReadInt(TMP_InputField input, out int value)
    {
        value = 0;
        if (input == null || string.IsNullOrWhiteSpace(input.text)) return false;

        return int.TryParse(input.text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    private bool Fail(string message, out string error)
    {
        error = message;
        return false;
    }
}