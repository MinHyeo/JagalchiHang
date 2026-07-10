using System;
using UnityEngine;

public class TimeManager : SingletonBase<TimeManager>
{
    [SerializeField] private float _timeScale = 60f;

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
}
