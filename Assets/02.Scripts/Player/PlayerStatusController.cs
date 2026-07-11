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
    private int _maxHp;
    private int _maxHunger;
    private int _maxThirst;

    private int _hungerInterval;
    private int _thirstInterval;
    private int _hungerDecrease;
    private int _thirstDecrease;
    private int _hungerDamage;
    private int _thirstDamage;

    private int _curHp;
    private int _curHunger;
    private int _curThirst;

    public int MaxHp => _maxHp;
    public int MaxHunger => _maxHunger;
    public int MaxThirst => _maxThirst;
    public int CurHp => _curHp;
    public int CurHunger => _curHunger;
    public int CurThirst => _curThirst;

    private PlayerController _playerController;

    public event Action PlayerStatusChanged;
    
    private void Start()
    {
        _playerController = GetComponent<PlayerController>();

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
        _maxHp = playerData.MaxHp;
        _maxHunger = playerData.MaxHunger;
        _maxThirst = playerData.MaxThirst;

        _hungerInterval = playerData.HungerInterval;
        _thirstInterval = playerData.ThirstInterval;
        _hungerDecrease = playerData.HungerDecrease;
        _thirstDecrease = playerData.ThirstDecrease;
        _hungerDamage = playerData.HungerDamage;
        _thirstDamage = playerData.ThirstDamage;

        _curHp = _maxHp;
        _curHunger = _maxHunger;
        _curThirst = _maxThirst;

        OnPlayerStatusChanged();
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

        _curHp = Mathf.Max(0, _curHp - decreaseValue);
        Debug.Log($"플레이어의 Hp가 {decreaseValue}만큼 감소했다.    현재 Hp : {_curHp}");

        if(_curHp <= 0)
        {
            _playerController.Die();
        }
        else
        {
            _playerController.Hit();
        }

        OnPlayerStatusChanged();
    }

    public void DecreaseHunger()
    {
        _curHunger = Mathf.Max(0, _curHunger - _hungerDecrease);
        Debug.Log($"플레이어의 Hunger가 {_hungerDecrease}만큼 감소했다.    현재 Hunger : {_curHunger}");

        if(_curHunger <= 0)
        {
            DecreaseHp(DamageType.Hunger);
        }

        OnPlayerStatusChanged();
    }

    public void DecreaseThirst()
    {
        _curThirst = Mathf.Max(0, _curThirst - _thirstDecrease);
        Debug.Log($"플레이어의 Thirst가 {_thirstDecrease}만큼 감소했다.    현재 Thirst  : {_curThirst}");

        if (_curThirst <= 0)
        {
            DecreaseHp(DamageType.Thirst);
        }

        OnPlayerStatusChanged();
    }

    private void OnPlayerStatusChanged()
    {
        PlayerStatusChanged?.Invoke();
    }
}
