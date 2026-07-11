using UnityEngine;
using UnityEngine.UI;

public class TestPlayerStatus : MonoBehaviour
{
    [SerializeField] private Slider _sliderHp;
    [SerializeField] private Slider _sliderHunger;
    [SerializeField] private Slider _sliderThirst;

    [SerializeField] private Text _textHp;
    [SerializeField] private Text _textHunger;
    [SerializeField] private Text _textThirst;

    [SerializeField] private PlayerStatusController _playerStatusController;

    private void OnEnable()
    {
        _playerStatusController.PlayerStatusChanged += OnHpChanged;
        _playerStatusController.PlayerStatusChanged += OnHungerChanged;
        _playerStatusController.PlayerStatusChanged += OnThirstChanged;
    }

    private void OnHpChanged()
    {
        _sliderHp.value = (float)_playerStatusController.CurHp / _playerStatusController.MaxHp;
        _textHp.text = $"{_playerStatusController.CurHp} / {_playerStatusController.MaxHp}";
    }

    private void OnHungerChanged()
    {
        _sliderHunger.value = (float)_playerStatusController.CurHunger / _playerStatusController.MaxHunger;
        _textHunger.text = $"{_playerStatusController.CurHunger} / {_playerStatusController.MaxHunger}";
    }

    private void OnThirstChanged()
    {
        _sliderThirst.value = (float)_playerStatusController.CurThirst / _playerStatusController.MaxThirst;
        _textThirst.text = $"{_playerStatusController.CurThirst} / {_playerStatusController.MaxThirst}";
    }
}
