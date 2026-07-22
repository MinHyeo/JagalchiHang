using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerViewModel : ViewModelBase
{
    // [나라]TODO : 수정 필요
    private PlayerModel _model = new PlayerModel();

    public int MaxHp { get; set; }
    public int MaxHunger { get; set; }
    public int MaxThirst { get; set; }

    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(CurrentHp));
        OnPropertyChanged(nameof(CurrentHunger));
        OnPropertyChanged(nameof(CurrentThirst));
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
