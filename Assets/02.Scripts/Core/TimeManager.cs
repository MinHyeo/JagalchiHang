using System;
using UnityEngine;

public class TimeManager : SingletonBase<TimeManager>
{
    [Header("시간 설정")]
    [Tooltip("하루의 기준 시간을 설정(600 => 10분이 하루)")]
    [SerializeField] private float _fullDayTime = 600f;

    [Header("Sun")]
    [SerializeField] private Light _sun;
    [SerializeField] private Gradient _sunColor;
    [SerializeField] private AnimationCurve _sunIntenstiy;
    [Header("Moon")]
    [SerializeField] private Light _moon;
    [SerializeField] private Gradient _moonColor;
    [SerializeField] private AnimationCurve _moonIntenstiy;

    [Header("Other Lighting")]
    [SerializeField] private AnimationCurve _lightingIntensityMultiplier;
    [SerializeField] private AnimationCurve _reflectionIntensityMultiplier;

    private DNSkyBoxSwitcher _skyBoxSwitcher;

    public int Day => _day;
    public int Hour => (int)(_time * 24f) ;
    public int Minute => (int)((_time * 1440f) % 60f);

    private int _day;
    private float _time;
    private float _startTime = 0.4f;
    private float _timeRate;
    private Vector3 _noon = new Vector3(90, 0, 0);

    public event Action OnMinuteChanged;
    public event Action OnHourChanged;
    public event Action OnDayChanged;

    private void OnEnable()
    {
        _skyBoxSwitcher = GetComponentInChildren<DNSkyBoxSwitcher>();

        _day = 0;
        _timeRate = 1.0f / _fullDayTime;
        _time = _startTime;

        OnHourChanged += HandleHourChanged;
        HandleHourChanged();
    }

    private void Update()
    {
        UpdateTime();

        UpdateLighting(_sun, _sunColor, _sunIntenstiy);
        UpdateLighting(_moon, _moonColor, _moonIntenstiy);

        RenderSettings.ambientIntensity = _lightingIntensityMultiplier.Evaluate(_time);
        RenderSettings.reflectionIntensity = _reflectionIntensityMultiplier.Evaluate(_time);
    }

    private void UpdateTime()
    {
        int prevHour = Hour;
        int prevMinute = Minute;

        _time = _time + _timeRate * Time.deltaTime;

        if(_time >= 1.0f)
        {
            int passedDays = (int)_time;
            _day += passedDays;
            _time = _time % 1.0f;

            OnDayChanged?.Invoke();
        }

        if(Minute != prevMinute)
        {
            OnMinuteChanged?.Invoke();
        }

        if(Hour != prevHour)
        {
            OnHourChanged?.Invoke();
        }
    }

    private void UpdateLighting(Light light, Gradient gradient, AnimationCurve animationCurve)
    {
        if (light == null)
            return;

        float intensity = animationCurve.Evaluate(_time);
        Color color = gradient.Evaluate(_time);

        light.transform.eulerAngles = (_time - (light == _sun ? 0.25f : 0.75f)) * _noon * 4.0f;
        light.color = color;
        light.intensity = intensity;

        GameObject lightObject = light.gameObject;
        if(light.intensity == 0 && lightObject.activeInHierarchy == true)
        {
            lightObject.SetActive(false);
        }
        else if(light.intensity > 0 && lightObject.activeInHierarchy == false)
        {
            lightObject.SetActive(true);
        }
    }

    public void RestartTime()
    {
        Time.timeScale = 1.0f;
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    private void HandleHourChanged()
    {
        int currentHour = Hour;

        if (currentHour >= 6 && currentHour < 10)
        {
            _skyBoxSwitcher.ChangeSkybox(DNSkyboxType.Morning);
        }
        else if (currentHour >= 10 && currentHour < 17)
        {
            _skyBoxSwitcher.ChangeSkybox(DNSkyboxType.Day);
        }
        else if (currentHour >= 17 && currentHour < 20)
        {
            _skyBoxSwitcher.ChangeSkybox(DNSkyboxType.Dusk);
        }
        else
        {
            _skyBoxSwitcher.ChangeSkybox(DNSkyboxType.Night);
        }
    }
}