using System;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Monster,
    Hunger,
    Thirst
}

public class PlayerStatusController : MonoBehaviour, IPlayerDamageable
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
        _vm = NetworkManager.Instance.PlayerService.GetPlayerViewModel();
        _playerController = GetComponent<PlayerController>();

        var player = this.GetComponentInParent<Player>();
        if (player == null) return;

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

    public void TakeDamage(int attackPower)
    {
        DecreaseHp(DamageType.Monster, attackPower);
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

    private void DecreaseHp(DamageType damageType, int damageAmount)
    {
        int decreaseValue = 0;

        switch(damageType)
        {
            case DamageType.Monster:
                decreaseValue = damageAmount;
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

    private void DecreaseHunger()
    {
        _vm.CurrentHunger = Mathf.Max(0, _vm.CurrentHunger - _hungerDecrease);
        Debug.Log($"플레이어의 Hunger가 {_hungerDecrease}만큼 감소했다.    현재 Hunger : {_vm.CurrentHunger}");

        if(_vm.CurrentHunger <= 0)
        {
            DecreaseHp(DamageType.Hunger, _hungerDamage);
        }
    }

    private void DecreaseThirst()
    {
        _vm.CurrentThirst = Mathf.Max(0, _vm.CurrentThirst - _thirstDecrease);
        Debug.Log($"플레이어의 Thirst가 {_thirstDecrease}만큼 감소했다.    현재 Thirst  : {_vm.CurrentThirst}");

        if (_vm.CurrentThirst <= 0)
        {
            DecreaseHp(DamageType.Thirst, _thirstDamage);
        }
    }
}
