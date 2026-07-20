using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerStatusView : ViewBase
{
    [SerializeField] private Slider _sliderHp;
    [SerializeField] private Slider _sliderHunger;
    [SerializeField] private Slider _sliderThirst;

    [SerializeField] private Text _textHp;
    [SerializeField] private Text _textHunger;
    [SerializeField] private Text _textThirst;

    private PlayerViewModel _vm;

    // [나라]TODO : 플레이어 생성될 때 호출해주기
    public void BindViewModel(PlayerViewModel vm)
    {
        _vm = vm;
        _vm.PropertyChanged += OnPropertyChanged;
        _vm.InvokeOnceOnInit();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case nameof(PlayerViewModel.MaxHp):
            case nameof(PlayerViewModel.CurrentHp):
                UpdateHp();
                break;
            case nameof(PlayerViewModel.MaxHunger):
            case nameof(PlayerViewModel.CurrentHunger):
                UpdateHunger();
                break;
            case nameof(PlayerViewModel.MaxThirst):
            case nameof(PlayerViewModel.CurrentThirst):
                UpdateThirst();
                break;
        }
    }

    private void UpdateHp()
    {
        _sliderHp.value = (float)_vm.CurrentHp / _vm.MaxHp;
        _textHp.text = $"{_vm.CurrentHp} / {_vm.MaxHp}";
    }

    private void UpdateHunger()
    {
        _sliderHunger.value = (float)_vm.CurrentHunger / _vm.MaxHunger;
        _textHunger.text = $"{_vm.CurrentHunger} / {_vm.MaxHunger}";
    }

    private void UpdateThirst()
    {
        _sliderThirst.value = (float)_vm.CurrentThirst / _vm.MaxThirst;
        _textThirst.text = $"{_vm.CurrentThirst} / {_vm.MaxThirst}";
    }
}
