using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerViewModel : ViewModelBase
{
    // [나라]TODO : 수정 필요
    private PlayerModel _model = new PlayerModel();

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(MaxHp));
        OnPropertyChanged(nameof(CurrentHp));
        OnPropertyChanged(nameof(MaxHunger));
        OnPropertyChanged(nameof(CurrentHunger));
        OnPropertyChanged(nameof(MaxThirst));
        OnPropertyChanged(nameof(CurrentThirst));
    }

    private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;
        set
        {
            if(_maxHp != value)
            {
                _maxHp = value;
                OnPropertyChanged(nameof(MaxHp));
            }
        }
    }

    public int CurrentHp
    {
        get => _model.CurrentHp;
        set
        {
            if(_model.CurrentHp != value)
            {
                _model.CurrentHp = value;
                OnPropertyChanged(nameof(CurrentHp));
            }
        }
    }

    private int _maxHunger;
    public int MaxHunger
    {
        get => _maxHunger;
        set
        {
            if (_maxHunger != value)
            {
                _maxHunger = value;
                OnPropertyChanged(nameof(MaxHunger));
            }
        }
    }

    public int CurrentHunger
    {
        get => _model.CurrentHunger;
        set
        {
            if (_model.CurrentHunger != value)
            {
                _model.CurrentHunger = value;
                OnPropertyChanged(nameof(CurrentHunger));
            }
        }
    }

    private int _maxThirst;
    public int MaxThirst
    {
        get => _maxThirst;
        set
        {
            if (_maxThirst != value)
            {
                _maxThirst = value;
                OnPropertyChanged(nameof(MaxThirst));
            }
        }
    }

    public int CurrentThirst
    {
        get => _model.CurrentThirst;
        set
        {
            if (_model.CurrentThirst != value)
            {
                _model.CurrentThirst = value;
                OnPropertyChanged(nameof(CurrentThirst));
            }
        }
    }
}
