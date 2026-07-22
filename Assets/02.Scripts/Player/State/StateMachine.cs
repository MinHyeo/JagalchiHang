using System.Collections.Generic;

public enum StateType
{
    None,
    Idle,
    Walk,
    Run,
    Attack,
    Die,
    Hit,
    PickUp
}

public class StateMachine
{
    private Dictionary<StateType, IPlayerState> _stateDic = new Dictionary<StateType, IPlayerState>();

    private IPlayerState _currentState;

    public void AddState(StateType stateType, IPlayerState state)
    {
        _stateDic.Add(stateType, state);
    }

    public void SetState(StateType stateType, PlayerController player)
    {
        // 딕셔너리에 stateType가 없다면
        if (_stateDic.TryGetValue(stateType, out IPlayerState newState) == false)
        {
            return;
        }

        // 현재 상태가 있으면
        if (_currentState != null)
        {
            // 현재 상태 종료
            _currentState.Exit(player);
        }

        _currentState = newState;
        _currentState.Enter(player);
    }

    public void Update(PlayerController player)
    {
        if (_currentState != null)
        {
            _currentState.Update(player);
        }
    }
    
    public void FixedUpdate(PlayerController player)
    {
        if (_currentState != null)
        {
            _currentState.FixedUpdate(player);
        }
    }
}
