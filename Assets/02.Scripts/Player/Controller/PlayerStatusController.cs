using System;
using UnityEngine;

public enum DamageType
{
    Monster,
    Hunger,
    Thirst
}

public class PlayerStatusController : MonoBehaviour
{
    private int _hungerInterval;
    private int _thirstInterval;
    private int _hungerDecrease;
    private int _thirstDecrease;
    private int _hungerDamage;
    private int _thirstDamage;

    private PlayerController _playerController;
    private PlayerViewModel _vm;

    private void Awake()
    {
        //_vm = NetworkManager_re.Inst.PlayerService.GetPlayerViewModel();
        _playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged += OnHungerChanged;
            TimeManager.Instance.OnMinuteChanged += OnThirstChanged;
        }

        return;
    }

    private void Update()
    {
        TestStatusDecrease();
    }

    public void InitPlayerStatus(PlayerData playerData)
    {
        if (playerData == null) return;

        _vm.MaxHp = playerData.MaxHp;
        _vm.MaxHunger = playerData.MaxHunger;
        _vm.MaxThirst = playerData.MaxThirst;

        _hungerInterval = playerData.HungerInterval;
        _thirstInterval = playerData.ThirstInterval;
        _hungerDecrease = playerData.HungerDecrease;
        _thirstDecrease = playerData.ThirstDecrease;
        _hungerDamage = playerData.HungerDamage;
        _thirstDamage = playerData.ThirstDamage;

        _vm.CurrentHp = _vm.MaxHp;
        _vm.CurrentHunger = _vm.MaxHunger;
        _vm.CurrentThirst = _vm.MaxThirst;
    }

    private void TestStatusDecrease()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            DecreaseHp(DamageType.Monster);
        }
    }

    private void OnHungerChanged()
    {
        if(_playerController.IsDie == true)
        {
            return;
        }

        if (TimeManager.Instance.Minute % _hungerInterval == 0)
        {
            DecreaseHunger();
        }
    }

    private void OnThirstChanged()
    {
        if (_playerController.IsDie == true)
        {
            return;
        }

        if(TimeManager.Instance.Minute % _thirstInterval == 0)
        {
            DecreaseThirst();
        }
    }

    public void DecreaseHp(DamageType damageType)
    {
        int decreaseValue = 0;

        switch(damageType)
        {
            case DamageType.Monster:
                decreaseValue = 10;
                break;
            case DamageType.Hunger:
                decreaseValue = _hungerDamage;
                break;
            case DamageType.Thirst:
                decreaseValue = _thirstDamage;
                break;
            default:
                break;
        }

        _vm.CurrentHp = Mathf.Max(0, _vm.CurrentHp - decreaseValue);
        Debug.Log($"플레이어의 Hp가 {decreaseValue}만큼 감소했다.    현재 Hp : {_vm.CurrentHp}");

        if(_vm.CurrentHp <= 0)
        {
            _playerController.Die();
        }
        else
        {
            _playerController.Hit();
        }
    }

    public void DecreaseHunger()
    {
        _vm.CurrentHunger = Mathf.Max(0, _vm.CurrentHunger - _hungerDecrease);
        Debug.Log($"플레이어의 Hunger가 {_hungerDecrease}만큼 감소했다.    현재 Hunger : {_vm.CurrentHunger}");

        if(_vm.CurrentHunger <= 0)
        {
            DecreaseHp(DamageType.Hunger);
        }
    }

    public void DecreaseThirst()
    {
        _vm.CurrentThirst = Mathf.Max(0, _vm.CurrentThirst - _thirstDecrease);
        Debug.Log($"플레이어의 Thirst가 {_thirstDecrease}만큼 감소했다.    현재 Thirst  : {_vm.CurrentThirst}");

        if (_vm.CurrentThirst <= 0)
        {
            DecreaseHp(DamageType.Thirst);
        }
    }
}
