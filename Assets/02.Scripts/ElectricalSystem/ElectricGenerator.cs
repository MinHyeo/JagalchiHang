using UnityEngine;

public class ElectricGenerator : MonoBehaviour
{
    private bool _isBroken;

    private ParticleSystem _electricParticle;
    private RotateSubmarineBlades _rotateBlades;

    private int _maxPower = 100;
    private int _currentPower;

    private int _troublePower = 500;
    private int _currentTroblePower;

    private void Awake()
    {
        _electricParticle = GetComponent<ParticleSystem>();
        _rotateBlades = GetComponent<RotateSubmarineBlades>();
    }

    private void OnEnable()
    {
        _currentPower = _maxPower;
        _currentPower = 0;

        RotateBlade();
    }

    public int UseGenerator(int amount)
    {
        if (_rotateBlades.rotate == false)
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

        RotateBlade();

        Debug.Log($"발전기 사용 : {_currentPower}");
        return power;
    }

    private void BreakGenerator()
    {
        Debug.LogWarning("발전기 고장!!");
        _electricParticle.Play();
        _isBroken = true;
    }

    public void FixGenerator()
    {
        Debug.Log("발전기 수리");
        _currentTroblePower = 0;
        _electricParticle.Pause();
        _isBroken = false;
        RotateBlade();
    }

    public void ReCharageGenerator(int amount)
    {
        Debug.Log($"발전기 충전 : {_currentPower} => {Mathf.Min(_currentPower + amount, _maxPower)}");
        _currentPower = Mathf.Min(_currentPower + amount, _maxPower);
        RotateBlade();
    }

    private void RotateBlade()
    {
        _rotateBlades.rotate = _currentPower > 0 && _isBroken == false;
    }
}