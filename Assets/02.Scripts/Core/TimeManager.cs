using System;
using UnityEngine;

public class TimeManager : SingletonBase<TimeManager>
{
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

    public int Day;
    public int Hour;
    public int Minute;

    private float _time;
    private float _fullDayTime = 30f;
    private float _startTime = 0.4f;
    private float _timeRate;
    private Vector3 _noon = new Vector3(90, 0, 0);

    public event Action OnMinuteChanged;
    public event Action OnHourChanged;
    public event Action OnDayChanged;

    private void OnEnable()
    {
        _timeRate = 1.0f / _fullDayTime;
        _time = _startTime;
    }

    private void Update()
    {
        _time = (_time + _timeRate * Time.deltaTime) % 1.0f;

        UpdateLighting(_sun, _sunColor, _sunIntenstiy);
        UpdateLighting(_moon, _moonColor, _moonIntenstiy);

        RenderSettings.ambientIntensity = _lightingIntensityMultiplier.Evaluate(_time);
        RenderSettings.reflectionIntensity = _reflectionIntensityMultiplier.Evaluate(_time);
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
}