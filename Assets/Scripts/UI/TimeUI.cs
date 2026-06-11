using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private Satellite _satellite = null;
    [SerializeField] private TMP_Text Speed = null;
    [SerializeField] private Slider Time = null;
    [SerializeField] private TMP_Text Day = null;
    [SerializeField] private TMP_Text Hour = null;
    [SerializeField] private TMP_Text Minute = null;
    [SerializeField] private TMP_Text Second = null;
    public void ChangeSpeed(float speed)
    {
        //_satellite.SpeedTime = (int)speed;
        Speed.text =  speed.ToString();
    }
    //public void ChangeTimeIndex(float speed) => _satellite.TimeIndex = (int)speed;
    public void UITime(int indexTime)
    {
        Time.value = indexTime;
        Day.text = (indexTime / 4320000).ToString() + " сут.";
        Hour.text = ((indexTime / 180000) % 24).ToString() + " ч.";
        Minute.text = ((indexTime / 3000) % 60).ToString() + " мин.";
        Second.text = ((indexTime / 50) % 60).ToString() + " сек.";
    }
    private void Start()
    {
        Time.maxValue = (int)Math.Round(Run.Instance.TimeEnd / 0.02, 0);
    }
}
