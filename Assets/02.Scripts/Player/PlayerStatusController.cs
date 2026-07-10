using UnityEngine;

public class PlayerStatusController : MonoBehaviour
{
    private int _maxHp;
    private int _maxHunger;
    private int _maxThirst;

    private int _curHp;
    private int _curHunger;
    private int _curThirst;
    
    private void Start()
    {
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
        DecreseHunger(1);
    }

    private void OnThirstChanged()
    {
        DecreseThirst(5);
    }

    public void DecreseHp(int value)
    {
        if(_curHp > 0)
        {
            _curHp -= value;
            Debug.Log($"플레이어의 Hp가 {value}만큼 감소했다.    현재 Hp : {_curHp}");
        }
    }

    public void DecreseHunger(int value)
    {
        if(_curHunger > 0)
        {
            _curHunger -= value;
            Debug.Log($"플레이어의 Hunger가 {value}만큼 감소했다.    현재 Hunger : {_curHunger}");
        }
    }

    public void DecreseThirst(int value)
    {
        if(_curThirst > 0)
        {
            _curThirst -= value;
            Debug.Log($"플레이어의 Thirst가 {value}만큼 감소했다.    현재 Hunger : {_curThirst}");

        }
    }
}
