using System.Globalization;
using TMPro;
using UnityEngine;

public sealed class IntegratorPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField keplerParam1Input;
    [SerializeField] private TMP_InputField keplerParam2Input;
    [SerializeField] private TMP_InputField keplerParam3Input;

    [SerializeField] private TMP_InputField rkParam1Input;
    [SerializeField] private TMP_InputField rkParam2Input;
    [SerializeField] private TMP_InputField rkParam3Input;

    public bool TryGetData(out IntegratorConfig data, out string error)
    {
        data = null;
        error = string.Empty;

        if (!TryReadDouble(keplerParam1Input, out double k1))
            return Fail("Параметр Кеплера 1 некорректен.", out error);
        if (!TryReadDouble(keplerParam2Input, out double k2))
            return Fail("Параметр Кеплера 2 некорректен.", out error);
        if (!TryReadDouble(keplerParam3Input, out double k3))
            return Fail("Параметр Кеплера 3 некорректен.", out error);

        if (!TryReadDouble(rkParam1Input, out double r1))
            return Fail("Параметр RK 1 некорректен.", out error);
        if (!TryReadDouble(rkParam2Input, out double r2))
            return Fail("Параметр RK 2 некорректен.", out error);
        if (!TryReadDouble(rkParam3Input, out double r3))
            return Fail("Параметр RK 3 некорректен.", out error);

        data = new IntegratorConfig(k1, k2, k3, r1, r2, r3);
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