using UnityEngine;

public class GeneratorViewModel : ViewModelBase
{
    private bool _isStoped;
    public bool IsStoped
    {
        get { return _isStoped; }
        set
        {
            if(_isStoped != value)
            {
                _isStoped = value;
                OnPropertyChanged(nameof(CurrentPower));
            }
        }
    }

    public int MaxPower;
    private int _currentPower;
    public int CurrentPower
    {
        get { return _currentPower; }
        set
        {
            if (_currentPower != value)
            {
                _currentPower = value;
                OnPropertyChanged(nameof(CurrentPower));
            }
        }
    }

    public int MaxTroublePower;
    private int _currentTroublePower;
    public int CurrentTroublePower
    {
        get { return _currentTroublePower; }
        set
        {
            if(_currentTroublePower != value)
            {
                _currentTroublePower = value;
                OnPropertyChanged(nameof(CurrentTroublePower));
            }
        }
    }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(CurrentPower));
        OnPropertyChanged(nameof(CurrentTroublePower));
        OnPropertyChanged(nameof(IsStoped));
    }
}