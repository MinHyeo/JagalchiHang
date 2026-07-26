using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private Text _dayText;
    [SerializeField] private Text _hourText;
    [SerializeField] private Text _minuteText;

    private void OnEnable()
    {
        ChangeDayText();
        ChangeHourText();
        ChangeMinuteText();

        TimeManager.Instance.OnDayChanged += ChangeDayText;
        TimeManager.Instance.OnHourChanged += ChangeHourText;
        TimeManager.Instance.OnMinuteChanged += ChangeMinuteText;
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnDayChanged -= ChangeDayText;
        TimeManager.Instance.OnHourChanged -= ChangeHourText;
        TimeManager.Instance.OnMinuteChanged -= ChangeMinuteText;
    }

    private void ChangeDayText()
    {
        _dayText.text = TimeManager.Instance.Day.ToString("D2") + 'D'; 
    }

    private void ChangeHourText()
    {
        _hourText.text = TimeManager.Instance.Hour.ToString("D2");
    }

    private void ChangeMinuteText()
    {
        _minuteText.text = TimeManager.Instance.Minute.ToString("D2");
    }
}
