using System;
using UnityEngine;

public class TimeManager : SingletonBase<TimeManager>
{
    [Header("시간 설정")]
    [SerializeField] private float _timeScale = 60f;
    [Header("Sun")]
    [SerializeField] private Light _sun;
    [SerializeField] private Gradient _sunColor;
    [SerializeField] private AnimationCurve _sunIntenstiy;
    [Header("Moon")]
    [SerializeField] private Light _moon;
    [SerializeField] private Gradient _moonColor;
    [SerializeField] private AnimationCurve _moonIntenstiy;

    private int _year = 2026;
    private int _month = 7;
    private int _day = 9;
    private int _hour = 11;
    private int _minute = 0;
    private float _second = 0;

    public int Year => _year;
    public int Month => _month;
    public int Day => _day;
    public int Hour => _hour;
    public int Minute => _minute;

    public event Action OnMinuteChanged;
    public event Action OnHourChanged;
    public event Action OnDayChanged;

    private void OnEnable()
    {
        OnMinuteChanged += UpdateDayNightCycle;
    }

    private void OnDisable()
    {
        OnMinuteChanged = null;
        OnHourChanged = null;
        OnDayChanged = null;
    }

    private void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        _second = _second + _timeScale * Time.deltaTime;

        if(_second >= 60)
        {
            int plusMinute = (int)_second / 60;
            _second %= 60;
            _minute = _minute + plusMinute;
            OnMinuteChanged?.Invoke();
        }

        if(_minute >= 60)
        {
            int plusHour = _minute / 60;
            _minute %= 60;
            _hour = _hour + plusHour;
            OnHourChanged?.Invoke();
        }

        if(_hour >= 24)
        {
            int plusDay = _hour / 24;
            _hour %= 24;
            _day = _day + plusDay;
            OnDayChanged?.Invoke();
        }
    }

    private void UpdateDayNightCycle()
    {
        UpdateLighting(_sun, _sunColor, _sunIntenstiy);
        UpdateLighting(_moon, _moonColor, _moonIntenstiy);
    }

    private void UpdateLighting(Light light, Gradient gradient, AnimationCurve animationCurve)
    {
        float time = ((_hour + 6) % 12 + Minute / 60.0f) / 12.0f;
        float intensity = _sunIntenstiy.Evaluate(time);
        Color color = _sunColor.Evaluate(time);


        float radius = ((360 / 24) * _hour - 90);
        radius += (15.0f / 60) * _minute;

        light.transform.rotation = Quaternion.Euler(radius, -30, 0);
        light.color = color;
        light.intensity = intensity;

        GameObject gameObject = light.gameObject;
        if(light.intensity == 0 && gameObject.activeInHierarchy == true)
        {
            gameObject.SetActive(false);
        }
        else if(light.intensity > 0 && gameObject.activeInHierarchy == false)
        {
            gameObject.SetActive(true);
        }
    }
}
