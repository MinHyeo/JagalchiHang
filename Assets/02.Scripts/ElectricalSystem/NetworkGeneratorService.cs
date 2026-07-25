using UnityEngine;

public class NetworkGeneratorService
{
    private GeneratorViewModel _generatorViewModel;

    public GeneratorViewModel GeneratorViewModel()
    {
        if (_generatorViewModel == null)
        {
            // 뷰모델 일단 생성
            var generatorViewModel = new GeneratorViewModel();
            SetGeneratorData(generatorViewModel);
            _generatorViewModel = generatorViewModel;
        }

        return _generatorViewModel;
    }

    private void SetGeneratorData(GeneratorViewModel vm)
    {
        vm.IsStoped = false;
        vm.CurrentPower = 100;
        vm.MaxPower = 200;
        vm.MaxTroublePower = 100;
        vm.CurrentTroublePower = 0;

    }

    // 발전기 사용 가능 여부 체크
    public bool CanUsePower(int amount)
    {
        if (_generatorViewModel == null)
        {
            return false;
        }

        if(_generatorViewModel.IsStoped || _generatorViewModel.CurrentPower < amount)
        {
            return false;
        }

        return true;
    }

    // 발전기 사용
    public int UsePower(int amount)
    {
        Debug.Log("dd");
        if (_generatorViewModel == null)
        {
            return 0;
        }

        int currentPower = _generatorViewModel.CurrentPower;
        int usePowerAmount = Mathf.Min(currentPower, amount);

        _generatorViewModel.CurrentPower = currentPower - usePowerAmount;
        _generatorViewModel.CurrentTroublePower = Mathf.Max(_generatorViewModel.CurrentTroublePower + usePowerAmount, _generatorViewModel.MaxTroublePower);
        CheckStopGenerator();

        Debug.Log($"사용량 : {amount}, 전력량 : {_generatorViewModel.CurrentPower}");
        return usePowerAmount;
    }

    // 발전기 충전
    public void ReChargePower(int amount)
    {
        if (_generatorViewModel == null)
        {
            return;
        }

        _generatorViewModel.CurrentPower = Mathf.Min(_generatorViewModel.CurrentPower + amount, _generatorViewModel.MaxPower);
        Debug.Log($"충전량 : {amount}, 충천 후 : {_generatorViewModel.CurrentPower}");
    }

    // 발전기 수리
    public void FixGenerator()
    {
        if(_generatorViewModel == null)
        {
            return;
        }

        if(_generatorViewModel.IsStoped == false)
        {
            return;
        }

        _generatorViewModel.IsStoped = false;
        _generatorViewModel.CurrentTroublePower = 0;
        Debug.Log("수리");
    }

    // 발전기 멈췄는지 체크
    private void CheckStopGenerator()
    {
        if(_generatorViewModel == null)
        {
            return;
        }

        if(_generatorViewModel.CurrentTroublePower < _generatorViewModel.MaxTroublePower)
        {
            return;
        }

        if(_generatorViewModel.CurrentPower > 0)
        {
            return;
        }

        Debug.Log("고장");
        _generatorViewModel.IsStoped = true;
    }
}