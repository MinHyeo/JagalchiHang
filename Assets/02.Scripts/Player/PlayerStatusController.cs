using UnityEngine;

public class PlayerStatusController : MonoBehaviour
{
    private int _maxHp;
    private int _maxHunger;
    private int _maxThirst;

    private int _curHp;
    private int _curHunger;
    private int _curThirst;

    private PlayerController _playerController;
    
    private void Start()
    {
        _playerController = GetComponent<PlayerController>();

        TimeManager.Instance.OnMinuteChanged += OnHungerChanged;
        TimeManager.Instance.OnHourChanged += OnThirstChanged;
    }

    private void Update()
    {
        TestStatusDecrese();
    }

    public void InitPlayerStatus(PlayerData playerData)
    {
        _maxHp = playerData.MaxHp;
        _maxHunger = playerData.MaxHunger;
        _maxThirst = playerData.MaxThirst;

        _curHp = _maxHp;
        _curHunger = _maxHunger;
        _curThirst = _maxThirst;
    }

    private void TestStatusDecrese()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            DecreseHp(10);
        }
    }

    private void OnHungerChanged()
    {
        if(_playerController.IsDie == true)
        {
            return;
        }

        DecreseHunger(1);
    }

    private void OnThirstChanged()
    {
        if (_playerController.IsDie == true)
        {
            return;
        }

        DecreseThirst(5);
    }

    public void DecreseHp(int value)
    {
        _playerController.Hit();

        _curHp -= value;
        Debug.Log($"플레이어의 Hp가 {value}만큼 감소했다.    현재 Hp : {_curHp}");

        if(_curHp <= 0)
        {
            _playerController.Die();
        }
    }

    public void DecreseHunger(int value)
    {
        _curHunger -= value;
        Debug.Log($"플레이어의 Hunger가 {value}만큼 감소했다.    현재 Hunger : {_curHunger}");

        if(_curHunger <= 0)
        {
            DecreseHp(5);
        }
    }

    public void DecreseThirst(int value)
    {
        _curThirst -= value;
        Debug.Log($"플레이어의 Thirst가 {value}만큼 감소했다.    현재 Hunger : {_curThirst}");

        if (_curThirst <= 0)
        {
            DecreseHp(5);
        }
    }

}
