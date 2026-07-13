using UnityEngine;

public class ElectricGenerator : MonoBehaviour
{
    private bool _isBroken = false;

    private int _maxPower = 100;
    private int _currentPower;

    private int _troublePower = 500;
    private int _currentTroblePower;

    private void OnEnable()
    {
        _currentPower = _maxPower;
        _currentPower = 0;
    }

    public int UseGenerator(int amount)
    {
        if (_isBroken)
            return 0;

        int power = amount;

        if(_currentPower < power)
        {
            power = _currentPower;
        }

        if(_currentTroblePower + power >= _troublePower)
        {
            power = power - ((_currentTroblePower + power) - _troublePower);
            BreakGenerator();
        }
        _currentPower = _currentPower - power;
        _currentTroblePower = _currentTroblePower + power;

        Debug.Log($"발전기 사용 : {_currentPower}");
        return power;
    }

    private void BreakGenerator()
    {
        Debug.LogWarning("발전기 고장!!");
        _isBroken = true;
    }

    public void FixGenerator()
    {
        Debug.Log("발전기 수리");
        _isBroken = false;
        _currentTroblePower = 0;
    }

    public void ReCharageGenerator(int amount)
    {
        Debug.Log($"발전기 충전 : {_currentPower} => {Mathf.Min(_currentPower + amount, _maxPower)}");
        _currentPower = Mathf.Min(_currentPower + amount, _maxPower);
    }
}