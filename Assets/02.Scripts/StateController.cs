using System;
using UnityEngine;

public enum EntityAnimState
{
    Idle,
    Walk,
    Run,
    Attack
}

public class StateController : MonoBehaviour
{
    [SerializeField] private Animator Animator_Entity;

    private EntityAnimState _currentState;

    public void SetState(EntityAnimState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        switch(_currentState)
        {
            case EntityAnimState.Idle:
                ResetState();
                break;
            case EntityAnimState.Walk:
                ResetState();
                Animator_Entity.SetBool("IsWalk", true);
                break;
            case EntityAnimState.Run:
                ResetState();
                Animator_Entity.SetBool("IsRun", true);
                break;
            case EntityAnimState.Attack:
                ResetState();
                Animator_Entity.SetBool("IsAttack", true);
                break;
            default:
                ResetState();
                break;

        }
    }

    private void ResetState()
    {
        Animator_Entity.SetBool("IsWalk", false);
        Animator_Entity.SetBool("IsRun", false);
        Animator_Entity.SetBool("IsAttack", false);
    }
}
